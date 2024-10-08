using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceContentTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceContentTypeCommandHandler _handler;

  private readonly ContentType _contentType = new(new Identifier("BlogArticle"));

  public CreateOrReplaceContentTypeCommandHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object, _contentTypeRepository.Object, _sender.Object);

    _contentTypeRepository.Setup(x => x.LoadAsync(_contentType.Id, _cancellationToken)).ReturnsAsync(_contentType);
  }

  [Theory(DisplayName = "It should create a new content type.")]
  [InlineData(null)]
  [InlineData("eab4ee0a-af70-4393-b850-0ad30d76e4b5")]
  public async Task It_should_create_a_new_content_type(string? idValue)
  {
    bool idParsed = Guid.TryParse(idValue, out Guid id);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<ContentType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new("BlogAuthor")
    {
      IsInvariant = true,
      DisplayName = " Blog Author ",
      Description = "    "
    };
    CreateOrReplaceContentTypeCommand command = new(id, payload, Version: null);
    command.Contextualize();

    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.ContentType);
    Assert.True(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentTypeCommand>(y => (!idParsed || y.ContentType.Id.ToGuid() == id)
        && y.ContentType.IsInvariant == payload.IsInvariant
        && y.ContentType.UniqueName.Value == payload.UniqueName
        && y.ContentType.DisplayName != null && y.ContentType.DisplayName.Value == payload.DisplayName.Trim()
        && y.ContentType.Description == null),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing content type.")]
  public async Task It_should_replace_an_existing_content_type()
  {
    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(_contentType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new("BlogArticle")
    {
      IsInvariant = true,
      DisplayName = " Blog Article ",
      Description = "    "
    };
    CreateOrReplaceContentTypeCommand command = new(_contentType.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.ContentType);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentTypeCommand>(y => y.ContentType.Equals(_contentType)
        && !y.ContentType.IsInvariant
        && y.ContentType.UniqueName.Value == payload.UniqueName
        && y.ContentType.DisplayName != null && y.ContentType.DisplayName.Value == payload.DisplayName.Trim()
        && y.ContentType.Description == null),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return an empty result when updating a content type that does not exist.")]
  public async Task It_should_return_an_empty_result_when_updating_a_content_type_that_does_not_exist()
  {
    CreateOrReplaceContentTypePayload payload = new("BlogAuthor");
    CreateOrReplaceContentTypeCommand command = new(Guid.NewGuid(), payload, Version: 1);

    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.ContentType);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(It.IsAny<SaveContentTypeCommand>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceContentTypePayload payload = new("123_BlogArticle");
    CreateOrReplaceContentTypeCommand command = new(Id: null, payload, Version: null);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("IdentifierValidator", error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing content type.")]
  public async Task It_should_update_an_existing_content_type()
  {
    long version = _contentType.Version;

    ContentType reference = new(_contentType.UniqueName, _contentType.IsInvariant, _contentType.CreatedBy, _contentType.Id);
    _contentTypeRepository.Setup(x => x.LoadAsync(_contentType.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("This is the content type for blog articles.");
    _contentType.Description = description;
    _contentType.Update();

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(_contentType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new("BlogArticle")
    {
      IsInvariant = true,
      DisplayName = " Blog Article ",
      Description = "    "
    };
    CreateOrReplaceContentTypeCommand command = new(_contentType.Id.ToGuid(), payload, version);
    command.Contextualize();

    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.ContentType);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentTypeCommand>(y => y.ContentType.Equals(_contentType)
        && !y.ContentType.IsInvariant
        && y.ContentType.UniqueName.Value == payload.UniqueName
        && y.ContentType.DisplayName != null && y.ContentType.DisplayName.Value == payload.DisplayName.Trim()
        && y.ContentType.Description == description),
      _cancellationToken), Times.Once);
  }
}
