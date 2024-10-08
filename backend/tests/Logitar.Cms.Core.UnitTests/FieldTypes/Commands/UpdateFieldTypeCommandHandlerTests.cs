using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateFieldTypeCommandHandler _handler;

  private readonly FieldType _fieldType = new("contents", new TextProperties());

  public UpdateFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object, _fieldTypeRepository.Object, _sender.Object);

    _fieldTypeRepository.Setup(x => x.LoadAsync(_fieldType.Id, _cancellationToken)).ReturnsAsync(_fieldType);
  }

  [Fact(DisplayName = "It should return null when the field type could not be found.")]
  public async Task It_should_return_null_when_the_field_type_could_not_be_found()
  {
    UpdateFieldTypePayload payload = new();
    UpdateFieldTypeCommand command = new(Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateFieldTypePayload payload = new()
    {
      UniqueName = "info@test.com"
    };
    UpdateFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("AllowedCharactersValidator", error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing field_type.")]
  public async Task It_should_update_an_existing_field_type()
  {
    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_fieldType, _cancellationToken)).ReturnsAsync(model);

    UpdateFieldTypePayload payload = new()
    {
      DisplayName = new Change<string>(" Contents "),
      Description = new Change<string>("    ")
    };
    UpdateFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload);
    command.Contextualize();

    FieldTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.NotNull(payload.DisplayName.Value);
    _sender.Verify(x => x.Send(
      It.Is<SaveFieldTypeCommand>(y => y.FieldType.Equals(_fieldType) && y.FieldType.UniqueName.Value == "contents"
        && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Value.Trim()
        && y.FieldType.Description == null),
      _cancellationToken), Times.Once);
  }
}
