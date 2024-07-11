using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.FieldTypes;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateFieldTypeCommandHandler _handler;

  public CreateFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateFieldTypePayload payload = new("ArticleTitle");
    CreateFieldTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotNullValidator", error.ErrorCode);
    Assert.Equal("StringProperties", error.PropertyName);
  }
}
