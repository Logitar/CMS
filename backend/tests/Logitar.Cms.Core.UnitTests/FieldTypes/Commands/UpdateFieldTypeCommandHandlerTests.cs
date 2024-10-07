using FluentValidation.Results;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
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

  public UpdateFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object, _fieldTypeRepository.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the field type is not found.")]
  public async Task It_should_return_null_when_the_field_type_is_not_found()
  {
    UpdateFieldTypePayload payload = new();
    UpdateFieldTypeCommand command = new(Guid.Empty, payload);
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UpdateFieldTypePayload payload = new()
    {
      StringProperties = new StringProperties(),
      TextProperties = new TextProperties()
    };
    UpdateFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NullValidator", error.ErrorCode);
    Assert.Equal("TextProperties", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing field type without version.")]
  public async Task It_should_update_an_existing_field_type_without_version()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UpdateFieldTypePayload payload = new()
    {
      DisplayName = new Change<string>(" Article Title "),
      Description = new Change<string>("    "),
      StringProperties = new StringProperties(minimumLength: 1, maximumLength: 100, pattern: null)
    };
    UpdateFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.Id == fieldType.Id
      && y.FieldType.UniqueName == fieldType.UniqueName
      && y.FieldType.DisplayName != null && payload.DisplayName.Value != null && y.FieldType.DisplayName.Value == payload.DisplayName.Value.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.String
      && ((ReadOnlyStringProperties)y.FieldType.Properties).MinimumLength == payload.StringProperties.MinimumLength
      && ((ReadOnlyStringProperties)y.FieldType.Properties).MaximumLength == payload.StringProperties.MaximumLength
      && ((ReadOnlyStringProperties)y.FieldType.Properties).Pattern == payload.StringProperties.Pattern
    ), _cancellationToken), Times.Once());

    _fieldTypeRepository.Verify(x => x.LoadAsync(fieldType.Id, It.IsAny<long>(), _cancellationToken), Times.Never);
  }
}
