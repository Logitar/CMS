﻿using FluentValidation.Results;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Properties;
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

  [Fact(DisplayName = "It should create a new Boolean field type.")]
  public async Task It_should_create_a_new_Boolean_field_type()
  {
    CreateFieldTypePayload payload = new("IsFeatured")
    {
      DisplayName = " Is featured? ",
      Description = "    ",
      BooleanProperties = new BooleanProperties()
    };
    CreateFieldTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.Boolean
      && y.FieldType.Properties is ReadOnlyBooleanProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should create a new DateTime field type.")]
  public async Task It_should_create_a_new_DateTime_field_type()
  {
    CreateFieldTypePayload payload = new("PublishedOn")
    {
      DisplayName = " Published on ",
      Description = "    ",
      DateTimeProperties = new DateTimeProperties(minimumValue: DateTime.UtcNow, maximumValue: null)
    };
    CreateFieldTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.DateTime
      && y.FieldType.Properties is ReadOnlyDateTimeProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should create a new Number field type.")]
  public async Task It_should_create_a_new_Number_field_type()
  {
    CreateFieldTypePayload payload = new("Price")
    {
      DisplayName = " Price ",
      Description = "    ",
      NumberProperties = new NumberProperties(minimumValue: 0.01, maximumValue: null, step: 0.01)
    };
    CreateFieldTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.Number
      && y.FieldType.Properties is ReadOnlyNumberProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should create a new String field type.")]
  public async Task It_should_create_a_new_String_field_type()
  {
    CreateFieldTypePayload payload = new("ArticleTitle")
    {
      DisplayName = " Article Title ",
      Description = "    ",
      StringProperties = new StringProperties(minimumLength: 1, maximumLength: 100, pattern: null)
    };
    CreateFieldTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.String
      && y.FieldType.Properties is ReadOnlyStringProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should create a new Text field type.")]
  public async Task It_should_create_a_new_Text_field_type()
  {
    CreateFieldTypePayload payload = new("ArticleContent")
    {
      DisplayName = " Article Content ",
      Description = "    ",
      TextProperties = new TextProperties(TextProperties.ContentTypes.PlainText, minimumLength: 1, maximumLength: 10000)
    };
    CreateFieldTypeCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveFieldTypeCommand>(y => y.FieldType.UniqueName.Value == payload.UniqueName
      && y.FieldType.DisplayName != null && y.FieldType.DisplayName.Value == payload.DisplayName.Trim()
      && y.FieldType.Description == null
      && y.FieldType.DataType == DataType.Text
      && y.FieldType.Properties is ReadOnlyTextProperties
    ), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateFieldTypePayload payload = new("ArticleTitle");
    CreateFieldTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("CreateFieldTypeValidator", error.ErrorCode);
  }
}
