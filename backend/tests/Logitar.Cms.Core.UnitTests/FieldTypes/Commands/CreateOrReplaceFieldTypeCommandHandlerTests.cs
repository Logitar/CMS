using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceFieldTypeCommandHandler _handler;

  private readonly FieldType _fieldType = new("Contents", new TextProperties(MediaTypeNames.Text.Plain, minimumLength: null, maximumLength: null));

  public CreateOrReplaceFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object, _fieldTypeRepository.Object, _sender.Object);

    _fieldTypeRepository.Setup(x => x.LoadAsync(_fieldType.Id, _cancellationToken)).ReturnsAsync(_fieldType);
  }

  [Theory(DisplayName = "It should create a new field type.")]
  [InlineData(null)]
  [InlineData("e60a6110-0702-4ebf-925a-c434fed25458")]
  public async Task It_should_create_a_new_field_type(string? idValue)
  {
    bool idParsed = Guid.TryParse(idValue, out Guid id);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new("title")
    {
      DisplayName = " Title ",
      Description = "    ",
      StringProperties = new()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    command.Contextualize();

    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.FieldType);
    Assert.True(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveFieldTypeCommand>(y => (!idParsed || y.FieldType.Id.ToGuid() == id)
        && y.FieldType.UniqueName.Value == payload.UniqueName
        && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
        && y.FieldType.Description == null
        && y.FieldType.DataType == DataType.String),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing field type.")]
  public async Task It_should_replace_an_existing_field_type()
  {
    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new("body")
    {
      DisplayName = " Body ",
      Description = "    ",
      TextProperties = new()
      {
        ContentType = MediaTypeNames.Text.Plain
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.FieldType);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveFieldTypeCommand>(y => y.FieldType.Equals(_fieldType)
        && y.FieldType.UniqueName.Value == payload.UniqueName
        && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
        && y.FieldType.Description == null
        && y.FieldType.DataType == DataType.Text),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return an empty result when updating a field type that does not exist.")]
  public async Task It_should_return_an_empty_result_when_updating_a_field_type_that_does_not_exist()
  {
    CreateOrReplaceFieldTypePayload payload = new("Title");
    CreateOrReplaceFieldTypeCommand command = new(Guid.NewGuid(), payload, Version: 1);

    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.FieldType);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(It.IsAny<SaveFieldTypeCommand>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceFieldTypePayload payload = new();
    CreateOrReplaceFieldTypeCommand command = new(Id: null, payload, Version: null);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CreateOrReplaceFieldTypeValidator");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueName");
  }

  [Fact(DisplayName = "It should update an existing field type.")]
  public async Task It_should_update_an_existing_field_type()
  {
    long version = _fieldType.Version;

    FieldType reference = new(_fieldType.UniqueName.Value, _fieldType.Properties, _fieldType.CreatedBy, _fieldType.Id);
    _fieldTypeRepository.Setup(x => x.LoadAsync(_fieldType.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("This is the article contents.");
    _fieldType.Description = description;
    _fieldType.Update();

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new("body")
    {
      DisplayName = " Body ",
      Description = "    ",
      TextProperties = new()
      {
        ContentType = MediaTypeNames.Text.Plain
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload, version);
    command.Contextualize();

    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.FieldType);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveFieldTypeCommand>(y => y.FieldType.Equals(_fieldType)
        && y.FieldType.UniqueName.Value == payload.UniqueName
        && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
        && y.FieldType.Description == description
        && y.FieldType.DataType == DataType.Text),
      _cancellationToken), Times.Once);
  }
}
