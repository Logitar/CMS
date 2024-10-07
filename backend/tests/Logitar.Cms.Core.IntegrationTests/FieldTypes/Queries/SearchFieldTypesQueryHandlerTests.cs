using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.FieldTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchFieldTypesQueryHandlerTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _contents;
  private readonly FieldTypeAggregate _metaTitle;
  private readonly FieldTypeAggregate _slug;
  private readonly FieldTypeAggregate _subTitle;
  private readonly FieldTypeAggregate _title;

  public SearchFieldTypesQueryHandlerTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;
    _contents = new(new UniqueNameUnit(uniqueNameSettings, "ArticleContents"), new ReadOnlyTextProperties());
    _metaTitle = new(new UniqueNameUnit(uniqueNameSettings, "ArticleMetaTitle"), new ReadOnlyStringProperties());
    _slug = new(new UniqueNameUnit(uniqueNameSettings, "Slug"), new ReadOnlyStringProperties());
    _subTitle = new(new UniqueNameUnit(uniqueNameSettings, "ArticleSubTitle"), new ReadOnlyStringProperties());
    _title = new(new UniqueNameUnit(uniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_contents, _metaTitle, _slug, _subTitle, _title]);
  }

  [Fact(DisplayName = "It should return empty results when no field type matches.")]
  public async Task It_should_return_empty_results_when_no_field_type_matches()
  {
    SearchFieldTypesPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };
    SearchFieldTypesQuery query = new(payload);

    SearchResults<FieldType> results = await Pipeline.ExecuteAsync(query);

    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct matching field types.")]
  public async Task It_should_return_the_correct_matching_field_types()
  {
    SearchFieldTypesPayload payload = new()
    {
      DataType = DataType.String,
      IdIn = (await _fieldTypeRepository.LoadAsync()).Select(fieldType => fieldType.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("%title"), new SearchTerm("con%")], SearchOperator.Or),
      Sort = [new FieldTypeSortOption(FieldTypeSort.UniqueName, isDescending: false)],
      Skip = 1,
      Limit = 1
    };
    payload.IdIn.Add(Guid.Empty);
    payload.IdIn.Remove(_metaTitle.Id.ToGuid());
    SearchFieldTypesQuery query = new(payload);

    SearchResults<FieldType> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    FieldType fieldType = Assert.Single(results.Items);
    Assert.Equal(_title.Id.ToGuid(), fieldType.Id);
  }
}
