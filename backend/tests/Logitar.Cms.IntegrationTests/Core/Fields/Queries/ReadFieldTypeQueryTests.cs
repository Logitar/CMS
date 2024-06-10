using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Fields.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadFieldTypeQueryTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _author;
  private readonly FieldTypeAggregate _subTitle;

  public ReadFieldTypeQueryTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;
    _author = new(new UniqueNameUnit(uniqueNameSettings, "author"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: null, pattern: null))
    {
      DisplayName = new DisplayNameUnit("Author")
    };
    _author.Update();
    _subTitle = new(new UniqueNameUnit(uniqueNameSettings, "subTitle"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: 100, pattern: null))
    {
      DisplayName = new DisplayNameUnit("Sub-title")
    };
    _subTitle.Update();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_author, _subTitle]);
  }

  [Fact(DisplayName = "It should return null when the field type is not found.")]
  public async Task It_should_return_null_when_the_field_type_is_not_found()
  {
    ReadFieldTypeQuery query = new(Id: Guid.NewGuid(), UniqueName: "Test");
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the field type found by ID.")]
  public async Task It_should_return_the_field_type_found_by_Id()
  {
    ReadFieldTypeQuery query = new(_subTitle.Id.ToGuid(), UniqueName: null);
    FieldType? fieldType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(fieldType);
    Assert.Equal(_subTitle.Id.ToGuid(), fieldType.Id);
  }

  [Fact(DisplayName = "It should return the field type found by unique name.")]
  public async Task It_should_return_the_field_type_found_by_unique_name()
  {
    ReadFieldTypeQuery query = new(Id: null, _author.UniqueName.Value);
    FieldType? fieldType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(fieldType);
    Assert.Equal(_author.Id.ToGuid(), fieldType.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many field types are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_field_types_are_found()
  {
    ReadFieldTypeQuery query = new(_subTitle.Id.ToGuid(), _author.UniqueName.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<FieldType>>(async () => await Pipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
