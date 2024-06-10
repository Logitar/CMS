using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Fields.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchFieldTypesQueryTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _author;
  private readonly FieldTypeAggregate _metaDescription;
  private readonly FieldTypeAggregate _metaKeywords;
  private readonly FieldTypeAggregate _metaTitle;
  private readonly FieldTypeAggregate _subTitle;

  public SearchFieldTypesQueryTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;
    _author = new(new UniqueNameUnit(uniqueNameSettings, "author"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: null, pattern: null))
    {
      DisplayName = new DisplayNameUnit("Author")
    };
    _author.Update();
    _metaDescription = new(new UniqueNameUnit(uniqueNameSettings, "metaDescription"), new ReadOnlyTextProperties(MediaTypeNames.Text.Plain, minimumLength: 1, maximumLength: 160))
    {
      DisplayName = new DisplayNameUnit("Description (meta)")
    };
    _metaDescription.Update();
    _metaKeywords = new(new UniqueNameUnit(uniqueNameSettings, "metaKeywords"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: null, pattern: null))
    {
      DisplayName = new DisplayNameUnit("Keywords (meta)")
    };
    _metaKeywords.Update();
    _metaTitle = new(new UniqueNameUnit(uniqueNameSettings, "metaTitle"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: 80, pattern: null))
    {
      DisplayName = new DisplayNameUnit("Title (meta)")
    };
    _metaTitle.Update();
    _subTitle = new(new UniqueNameUnit(uniqueNameSettings, "subTitle"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: 100, pattern: null))
    {
      DisplayName = new DisplayNameUnit("Sub-title")
    };
    _subTitle.Update();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_author, _metaDescription, _metaKeywords, _metaTitle, _subTitle]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchFieldTypesPayload payload = new()
    {
      DataType = (DataType)(-1)
    };

    SearchFieldTypesQuery query = new(payload);
    SearchResults<FieldType> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchFieldTypesPayload payload = new()
    {
      DataType = DataType.String,
      Ids = (await _fieldTypeRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("%author%"), new SearchTerm("META%")], SearchOperator.Or),
      Sort = [new FieldTypeSortOption(FieldTypeSort.DisplayName, isDescending: false)],
      Skip = 1,
      Limit = 1
    };

    payload.Ids.Remove(_author.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchFieldTypesQuery query = new(payload);
    SearchResults<FieldType> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, results.Total);
    Assert.Equal(payload.Limit, results.Items.Count);
    Assert.Equal(_metaTitle.Id.ToGuid(), results.Items.Single().Id);
  }
}
