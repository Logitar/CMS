using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateFieldTypeCommandHandlerTests : IntegrationTests
{
  public CreateFieldTypeCommandHandlerTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new field type.")]
  public async Task It_should_create_a_new_field_type()
  {
    CreateFieldTypePayload payload = new("ArticleTitle")
    {
      DisplayName = " Article Title ",
      Description = "    ",
      StringProperties = new StringProperties
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    CreateFieldTypeCommand command = new(payload);
    FieldType fieldType = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(default, fieldType.Id);
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.NotEqual(default, fieldType.CreatedOn);
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.True(fieldType.CreatedOn < fieldType.UpdatedOn);

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Null(fieldType.Description);
    Assert.Equal(DataType.String, fieldType.DataType);
    Assert.Equal(payload.StringProperties, fieldType.StringProperties);
  }
}
