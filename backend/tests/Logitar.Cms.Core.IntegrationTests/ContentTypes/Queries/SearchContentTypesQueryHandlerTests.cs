using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchContentTypesQueryHandlerTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _article;
  private readonly ContentTypeAggregate _blog;
  private readonly ContentTypeAggregate _author;
  private readonly ContentTypeAggregate _magazine;
  private readonly ContentTypeAggregate _product;

  public SearchContentTypesQueryHandlerTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _article = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _blog = new(new IdentifierUnit("Blog"), isInvariant: false);
    _author = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
    _magazine = new(new IdentifierUnit("Magazine"), isInvariant: false);
    _product = new(new IdentifierUnit("Product"), isInvariant: false);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync([_article, _blog, _author, _magazine, _product]);
  }

  [Fact(DisplayName = "It should return empty results when no content type matches.")]
  public async Task It_should_return_empty_results_when_no_content_type_matches()
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

  [Fact(DisplayName = "It should return the correct matching content types.")]
  public async Task It_should_return_the_correct_matching_content_types()
  {
    SearchContentTypesPayload payload = new()
    {
      IsInvariant = false,
      IdIn = (await _contentTypeRepository.LoadAsync()).Select(contentType => contentType.Id.ToGuid()).ToList(),
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
    CmsContentType contentType = Assert.Single(results.Items);
    Assert.Equal(_article.Id.ToGuid(), contentType.Id);
  }
}
