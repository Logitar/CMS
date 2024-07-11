using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.FieldTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadFieldTypeQueryHandlerTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _fieldType;

  public ReadFieldTypeQueryHandlerTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    _fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync(_fieldType);
  }

  [Fact(DisplayName = "It should return the field type found by ID.")]
  public async Task It_should_return_the_field_type_found_by_Id()
  {
    ReadFieldTypeQuery query = new(_fieldType.Id.ToGuid(), UniqueName: null);
    FieldType? fieldType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(fieldType);
    Assert.Equal(_fieldType.Id.ToGuid(), fieldType.Id);
  }

  [Fact(DisplayName = "It should return the field type found by unique name.")]
  public async Task It_should_return_the_field_type_found_by_unique_name()
  {
    ReadFieldTypeQuery query = new(Id: null, $" {_fieldType.UniqueName.Value.ToLower()} ");
    FieldType? fieldType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(fieldType);
    Assert.Equal(_fieldType.Id.ToGuid(), fieldType.Id);
  }
}
