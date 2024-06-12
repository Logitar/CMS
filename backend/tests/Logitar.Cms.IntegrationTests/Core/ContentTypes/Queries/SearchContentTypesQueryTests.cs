using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchContentTypesQueryTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;
  private readonly ContentTypeAggregate _blogCategory;
  private readonly ContentTypeAggregate _blogMedia;
  private readonly ContentTypeAggregate _product;

  public SearchContentTypesQueryTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false)
    {
      DisplayName = new DisplayNameUnit("Blog Article")
    };
    _blogArticle.Update();
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true)
    {
      DisplayName = new DisplayNameUnit("Blog Author")
    };
    _blogAuthor.Update();
    _blogCategory = new(new IdentifierUnit("BlogCategory"), isInvariant: false)
    {
      DisplayName = new DisplayNameUnit("Blog Category")
    };
    _blogCategory.Update();
    _blogMedia = new(new IdentifierUnit("BlogMedia"), isInvariant: false)
    {
      DisplayName = new DisplayNameUnit("BlogMedia")
    };
    _blogMedia.Update();
    _product = new(new IdentifierUnit("Product"), isInvariant: false)
    {
      DisplayName = new DisplayNameUnit("Product")
    };
    _product.Update();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync([_blogArticle, _blogAuthor, _blogCategory, _blogMedia, _product]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchContentTypesPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };

    SearchContentTypesQuery query = new(payload);
    SearchResults<ContentsType> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchContentTypesPayload payload = new()
    {
      IsInvariant = false,
      Ids = (await _contentTypeRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("blog%")]),
      Sort = [new ContentTypeSortOption(ContentTypeSort.DisplayName, isDescending: true)],
      Skip = 1,
      Limit = 1
    };

    payload.Ids.Remove(_blogMedia.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchContentTypesQuery query = new(payload);
    SearchResults<ContentsType> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, results.Total);
    Assert.Equal(payload.Limit, results.Items.Count);
    Assert.Equal(_blogArticle.Id.ToGuid(), results.Items.Single().Id);
  }
}
