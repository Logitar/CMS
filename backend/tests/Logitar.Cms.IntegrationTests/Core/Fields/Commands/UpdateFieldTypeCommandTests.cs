using FluentValidation.Results;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateFieldTypeCommandTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _fieldType;

  public UpdateFieldTypeCommandTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    _fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "SubTitle"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: byte.MaxValue, pattern: null), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync(_fieldType);
  }

  [Fact(DisplayName = "It should return null when the field type could not be found.")]
  public async Task It_should_return_null_when_the_field_type_could_not_be_found()
  {
    UpdateFieldTypePayload payload = new();
    UpdateFieldTypeCommand command = new(Id: Guid.NewGuid(), payload);
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "Author"), new ReadOnlyStringProperties(), ActorId);
    await _fieldTypeRepository.SaveAsync(fieldType);

    UpdateFieldTypePayload payload = new()
    {
      UniqueName = fieldType.UniqueName.Value
    };
    UpdateFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<FieldTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateFieldTypePayload payload = new()
    {
      UniqueName = _fieldType.UniqueName.Value,
      StringProperties = new StringProperties
      {
        MinimumLength = 1,
        MaximumLength = byte.MaxValue,
        Pattern = null
      },
      TextProperties = new TextProperties(MediaTypeNames.Application.JsonPatch)
    };
    UpdateFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NullValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.TextProperties), error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing field type.")]
  public async Task It_should_update_an_existing_field_type()
  {
    _fieldType.Description = new DescriptionUnit("This is the field type for blog article sub-titles.");
    _fieldType.Update(ActorId);
    await _fieldTypeRepository.SaveAsync(_fieldType);

    UpdateFieldTypePayload payload = new()
    {
      DisplayName = new Modification<string>("  Sub-title  "),
      Description = new Modification<string>("    "),
      StringProperties = new StringProperties
      {
        MinimumLength = 1,
        MaximumLength = 128,
        Pattern = null
      }
    };
    UpdateFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload);
    FieldType? fieldType = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(fieldType);

    Assert.Equal(_fieldType.Id.ToGuid(), fieldType.Id);
    Assert.Equal(_fieldType.Version + 2, fieldType.Version);
    Assert.Equal(Contracts.Actors.Actor.System, fieldType.CreatedBy);
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.True(fieldType.CreatedOn < fieldType.UpdatedOn);

    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.CleanTrim(), fieldType.DisplayName);
    Assert.Null(fieldType.Description);
    Assert.Equal(_fieldType.DataType, fieldType.DataType);
    Assert.Equal(payload.StringProperties, fieldType.StringProperties);
    Assert.Null(fieldType.TextProperties);
  }
}
