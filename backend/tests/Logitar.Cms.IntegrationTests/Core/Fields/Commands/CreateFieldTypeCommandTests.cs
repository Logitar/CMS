using FluentValidation.Results;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateFieldTypeCommandTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateFieldTypeCommandTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
  }

  [Fact(DisplayName = "It should create a new boolean field type.")]
  public async Task It_should_create_a_new_boolean_field_type()
  {
    CreateFieldTypePayload payload = new("IsFeatured")
    {
      DisplayName = "  Is featured?  ",
      Description = "    ",
      BooleanProperties = new BooleanProperties()
    };
    CreateFieldTypeCommand command = new(payload);
    FieldType fieldType = await Pipeline.ExecuteAsync(command);

    await AssertIsValidAsync(payload, fieldType, DataType.Boolean);
  }

  [Fact(DisplayName = "It should create a new DateTime field type.")]
  public async Task It_should_create_a_new_DateTime_field_type()
  {
    CreateFieldTypePayload payload = new("PublicationDate")
    {
      DisplayName = "  Publication Date  ",
      Description = "    ",
      DateTimeProperties = new DateTimeProperties()
    };
    CreateFieldTypeCommand command = new(payload);
    FieldType fieldType = await Pipeline.ExecuteAsync(command);

    await AssertIsValidAsync(payload, fieldType, DataType.DateTime);
  }

  [Fact(DisplayName = "It should create a new number field type.")]
  public async Task It_should_create_a_new_number_field_type()
  {
    CreateFieldTypePayload payload = new("Diameter")
    {
      DisplayName = "  Diameter  ",
      Description = "    ",
      NumberProperties = new NumberProperties
      {
        MinimumValue = 6,
        MaximumValue = 24,
        Step = 1
      }
    };
    CreateFieldTypeCommand command = new(payload);
    FieldType fieldType = await Pipeline.ExecuteAsync(command);

    await AssertIsValidAsync(payload, fieldType, DataType.Number);
  }

  [Fact(DisplayName = "It should create a new string field type.")]
  public async Task It_should_create_a_new_string_field_type()
  {
    CreateFieldTypePayload payload = new("SubTitle")
    {
      DisplayName = "  Sub-title  ",
      Description = "    ",
      StringProperties = new StringProperties()
    };
    CreateFieldTypeCommand command = new(payload);
    FieldType fieldType = await Pipeline.ExecuteAsync(command);

    await AssertIsValidAsync(payload, fieldType, DataType.String);
  }

  [Fact(DisplayName = "It should create a new text field type.")]
  public async Task It_should_create_a_new_text_field_type()
  {
    CreateFieldTypePayload payload = new("Contents")
    {
      DisplayName = "  Page contents  ",
      Description = "    ",
      TextProperties = new TextProperties(MediaTypeNames.Text.Plain)
    };
    CreateFieldTypeCommand command = new(payload);
    FieldType fieldType = await Pipeline.ExecuteAsync(command);

    await AssertIsValidAsync(payload, fieldType, DataType.Text);
  }

  private async Task AssertIsValidAsync(CreateFieldTypePayload payload, FieldType fieldType, DataType dataType)
  {
    Assert.NotEqual(Guid.Empty, fieldType.Id);
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.True(fieldType.CreatedOn < fieldType.UpdatedOn);

    Assert.Equal(payload.UniqueName.Trim(), fieldType.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), fieldType.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), fieldType.Description);

    Assert.Equal(dataType, fieldType.DataType);
    Assert.Equal(payload.BooleanProperties, fieldType.BooleanProperties);
    Assert.Equal(payload.StringProperties, fieldType.StringProperties);
    Assert.Equal(payload.TextProperties, fieldType.TextProperties);

    FieldTypeEntity? entity = await CmsContext.FieldTypes.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(fieldType.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ReadOnlyStringProperties properties = new(minimumLength: 1, maximumLength: 255, pattern: null);
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "SubTitle"), properties);
    await _fieldTypeRepository.SaveAsync(fieldType);

    CreateFieldTypePayload payload = new(fieldType.UniqueName.Value)
    {
      StringProperties = new StringProperties()
    };
    CreateFieldTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<FieldTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateFieldTypePayload payload = new("SubTitle");
    CreateFieldTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(CreateFieldTypeValidator), error.ErrorCode);
    Assert.Equal(string.Empty, error.PropertyName);
  }
}
