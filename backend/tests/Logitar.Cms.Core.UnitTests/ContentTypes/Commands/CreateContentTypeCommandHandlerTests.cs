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
