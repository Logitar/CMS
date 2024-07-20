using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchContentTypesQueryHandlerTests : IntegrationTests
{
  private readonly IContentTypeRepository _fieldTypeRepository;

  private readonly ContentTypeAggregate _article;
  private readonly ContentTypeAggregate _blog;
  private readonly ContentTypeAggregate _author;
  private readonly ContentTypeAggregate _magazine;
  private readonly ContentTypeAggregate _product;

  public SearchContentTypesQueryHandlerTests() : base()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _article = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _blog = new(new IdentifierUnit("Blog"), isInvariant: false);
    _author = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
    _magazine = new(new IdentifierUnit("Magazine"), isInvariant: false);
    _product = new(new IdentifierUnit("Product"), isInvariant: false);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_article, _blog, _author, _magazine, _product]);
  }

  [Fact(DisplayName = "It should return empty results when no field type matches.")]
  public async Task It_should_return_empty_results_when_no_field_type_matches()
  {
    SearchContentTypesPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };
    SearchContentTypesQuery query = new(payload);

    SearchResults<CmsContentType> results = await Pipeline.ExecuteAsync(query);

    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct matching field types.")]
  public async Task It_should_return_the_correct_matching_field_types()
  {
    SearchContentTypesPayload payload = new()
    {
      IsInvariant = false,
      IdIn = (await _fieldTypeRepository.LoadAsync()).Select(fieldType => fieldType.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("blog%"), new SearchTerm("%z%")], SearchOperator.Or),
      Sort = [new ContentTypeSortOption(ContentTypeSort.UniqueName, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    payload.IdIn.Add(Guid.Empty);
    payload.IdIn.Remove(_blog.Id.ToGuid());
    SearchContentTypesQuery query = new(payload);

    SearchResults<CmsContentType> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    CmsContentType fieldType = Assert.Single(results.Items);
    Assert.Equal(_article.Id.ToGuid(), fieldType.Id);
  }
}
