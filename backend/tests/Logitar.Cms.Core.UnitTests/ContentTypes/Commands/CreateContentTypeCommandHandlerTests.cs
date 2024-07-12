using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateContentTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateContentTypeCommandHandler _handler;

  public CreateContentTypeCommandHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should create a new invariant content type.")]
  public async Task It_should_create_a_new_invariant_content_type()
  {
    CreateContentTypePayload payload = new("BlogArticle")
    {
      IsInvariant = true,
      DisplayName = " Blog Article ",
      Description = "    "
    };
    CreateContentTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentTypeCommand>(command => command.ContentType.IsInvariant
      && command.ContentType.UniqueName.Value == payload.UniqueName
      && command.ContentType.DisplayName != null && command.ContentType.DisplayName.Value == payload.DisplayName.Trim()
      && command.ContentType.Description == null
    ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new variant content type.")]
  public async Task It_should_create_a_new_variant_content_type()
  {
    CreateContentTypePayload payload = new("BlogAuthor");
    CreateContentTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentTypeCommand>(command => !command.ContentType.IsInvariant
      && command.ContentType.UniqueName.Value == payload.UniqueName
      && command.ContentType.DisplayName == null
      && command.ContentType.Description == null
    ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateContentTypePayload payload = new("123_BlogArticle");
    CreateContentTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("IdentifierValidator", error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }
}
