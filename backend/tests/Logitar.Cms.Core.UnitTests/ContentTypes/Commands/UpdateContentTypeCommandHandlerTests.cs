using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.ContentTypes;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateContentTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateContentTypeCommandHandler _handler;

  private readonly ContentType _contentType = new(new Identifier("blog_article"));

  public UpdateContentTypeCommandHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object, _contentTypeRepository.Object, _sender.Object);

    _contentTypeRepository.Setup(x => x.LoadAsync(_contentType.Id, _cancellationToken)).ReturnsAsync(_contentType);
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task It_should_return_null_when_the_content_type_could_not_be_found()
  {
    UpdateContentTypePayload payload = new();
    UpdateContentTypeCommand command = new(Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateContentTypePayload payload = new()
    {
      UniqueName = "123_BlogArticle"
    };
    UpdateContentTypeCommand command = new(_contentType.Id.ToGuid(), payload);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("IdentifierValidator", error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing content_type.")]
  public async Task It_should_update_an_existing_content_type()
  {
    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(_contentType, _cancellationToken)).ReturnsAsync(model);

    UpdateContentTypePayload payload = new()
    {
      UniqueName = "BlogArticle",
      DisplayName = new Change<string>(" Blog Article "),
      Description = new Change<string>("    ")
    };
    UpdateContentTypeCommand command = new(_contentType.Id.ToGuid(), payload);
    command.Contextualize();

    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.NotNull(payload.DisplayName.Value);
    _sender.Verify(x => x.Send(
      It.Is<SaveContentTypeCommand>(y => y.ContentType.Equals(_contentType) && y.ContentType.UniqueName.Value == payload.UniqueName
        && y.ContentType.DisplayName != null && y.ContentType.DisplayName.Value == payload.DisplayName.Value.Trim()
        && y.ContentType.Description == null),
      _cancellationToken), Times.Once);
  }
}
