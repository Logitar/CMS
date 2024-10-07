using FluentValidation.Results;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceFieldTypeCommandHandler _handler;

  public ReplaceFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object, _fieldTypeRepository.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing Boolean field type.")]
  public async Task It_should_replace_an_existing_Boolean_field_type()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "Featured"), new ReadOnlyBooleanProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ReplaceFieldTypePayload payload = new("IsFeatured")
    {
      DisplayName = " Is featured? ",
      Description = "    ",
      BooleanProperties = new BooleanProperties()
    };
    ReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, Version: null);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.Id == fieldType.Id
      && y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.Boolean
      && y.FieldType.Properties is ReadOnlyBooleanProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should replace an existing DateTime field type.")]
  public async Task It_should_replace_an_existing_DateTime_field_type()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "Published"), new ReadOnlyDateTimeProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ReplaceFieldTypePayload payload = new("PublishedOn")
    {
      DisplayName = " Published on ",
      Description = "    ",
      DateTimeProperties = new DateTimeProperties(minimumValue: DateTime.UtcNow, maximumValue: null)
    };
    ReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, Version: null);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.Id == fieldType.Id
      && y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.DateTime
      && y.FieldType.Properties is ReadOnlyDateTimeProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should replace an existing Number field type.")]
  public async Task It_should_replace_an_existing_Number_field_type()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "WordCount"), new ReadOnlyNumberProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ReplaceFieldTypePayload payload = new("Price")
    {
      DisplayName = " Price ",
      Description = "    ",
      NumberProperties = new NumberProperties(minimumValue: 0.01, maximumValue: null, step: 0.01)
    };
    ReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, Version: null);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.Id == fieldType.Id
      && y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.Number
      && y.FieldType.Properties is ReadOnlyNumberProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should replace an existing String field type.")]
  public async Task It_should_replace_an_existing_String_field_type()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "BlogTitle"), new ReadOnlyStringProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ReplaceFieldTypePayload payload = new("ArticleTitle")
    {
      DisplayName = " Article Title ",
      Description = "    ",
      StringProperties = new StringProperties(minimumLength: 1, maximumLength: 100, pattern: null)
    };
    ReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, Version: null);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.Id == fieldType.Id
      && y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.String
      && y.FieldType.Properties is ReadOnlyStringProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should replace an existing Text field type.")]
  public async Task It_should_replace_an_existing_Text_field_type()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "BlogContent"), new ReadOnlyTextProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ReplaceFieldTypePayload payload = new("ArticleContent")
    {
      DisplayName = " Article Content ",
      Description = "    ",
      TextProperties = new TextProperties(TextProperties.ContentTypes.PlainText, minimumLength: 1, maximumLength: 10000)
    };
    ReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, Version: null);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.Id == fieldType.Id
      && y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.Text
      && y.FieldType.Properties is ReadOnlyTextProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the field type is not found.")]
  public async Task It_should_return_null_when_the_field_type_is_not_found()
  {
    ReplaceFieldTypePayload payload = new();
    ReplaceFieldTypeCommand command = new(Guid.Empty, payload, Version: null);
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ReplaceFieldTypePayload payload = new("ArticleTitle")
    {
      StringProperties = new StringProperties(),
      TextProperties = new TextProperties()
    };
    ReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NullValidator", error.ErrorCode);
    Assert.Equal("TextProperties", error.PropertyName);
  }
}
