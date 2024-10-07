using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateFieldTypeCommandHandlerTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _fieldType;

  public UpdateFieldTypeCommandHandlerTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    _fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync(_fieldType);
  }

  [Fact(DisplayName = "It should update an existing field type.")]
  public async Task It_should_update_an_existing_field_type()
  {
    UpdateFieldTypePayload payload = new()
    {
      DisplayName = new Change<string>(" Article Title "),
      StringProperties = new StringProperties
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    UpdateFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload);
    FieldType? fieldType = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(fieldType);

    Assert.Equal(_fieldType.Id.ToGuid(), fieldType.Id);
    Assert.Equal(4, fieldType.Version);
    Assert.Equal(Contracts.Actors.Actor.System, fieldType.CreatedBy);
    Assert.Equal(_fieldType.CreatedOn.AsUniversalTime(), fieldType.CreatedOn);
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.True(fieldType.CreatedOn < fieldType.UpdatedOn);

    Assert.Equal(_fieldType.UniqueName.Value, fieldType.UniqueName);
    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.Trim(), fieldType.DisplayName);
    Assert.Null(fieldType.Description);
    Assert.Equal(DataType.String, fieldType.DataType);
    Assert.Null(fieldType.BooleanProperties);
    Assert.Null(fieldType.DateTimeProperties);
    Assert.Null(fieldType.NumberProperties);
    Assert.Equal(payload.StringProperties, fieldType.StringProperties);
    Assert.Null(fieldType.TextProperties);
  }
}
