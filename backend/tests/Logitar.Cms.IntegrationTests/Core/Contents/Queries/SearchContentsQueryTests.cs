using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchContentsQueryTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _canadianEnglish;
  private readonly LanguageAggregate _canadianFrench;

  private readonly ContentTypeAggregate _blogArticleType;
  private readonly ContentTypeAggregate _blogAuthorType;
  private readonly ContentTypeAggregate _blogCategoryType;

  private readonly ContentAggregate _blogAuthor;
  private readonly ContentAggregate _blogArticle1;
  private readonly ContentAggregate _blogArticle2;
  private readonly ContentAggregate _blogArticle3;
  private readonly ContentAggregate _blogArticle4;
  private readonly ContentAggregate _blogCategory;

  public SearchContentsQueryTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _canadianEnglish = new(new LocaleUnit("en-CA"));
    _canadianFrench = new(new LocaleUnit("fr-CA"));

    _blogArticleType = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _blogAuthorType = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
    _blogCategoryType = new(new IdentifierUnit("BlogCategory"), isInvariant: false);

    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;
    _blogAuthor = new(_blogAuthorType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "red-deer")));
    _blogArticle1 = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "prolongez-lete-avec-une-acura-nsx-coupe")));
    _blogArticle1.SetLocale(_canadianFrench, _blogArticle1.Invariant);
    _blogArticle2 = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "comparaison-acura-integra-2024-vs-bmw-serie-2-decouvrez-les-avantages-dacura")));
    _blogArticle2.SetLocale(_canadianFrench, _blogArticle2.Invariant);
    _blogArticle3 = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "decouvrir-acura-luxe-origine-et-durabilite-explores")));
    _blogArticle3.SetLocale(_canadianFrench, _blogArticle3.Invariant);
    _blogArticle4 = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "un-vus-electrisant-le-zdx-2024-debarque-chez-nous")));
    _blogArticle4.SetLocale(_canadianFrench, _blogArticle4.Invariant);
    _blogCategory = new(_blogCategoryType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "acura-news")));
    _blogCategory.SetLocale(_canadianEnglish, _blogCategory.Invariant);
    _blogCategory.SetLocale(_canadianFrench, _blogCategory.Invariant);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_canadianEnglish, _canadianFrench]);
    await _contentTypeRepository.SaveAsync([_blogArticleType, _blogAuthorType, _blogCategoryType]);
    await _contentRepository.SaveAsync([_blogAuthor, _blogArticle1, _blogArticle2, _blogArticle3, _blogArticle4, _blogCategory]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchContentsPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };

    SearchContentsQuery query = new(payload);
    SearchResults<ContentLocale> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results (invariant).")]
  public async Task It_should_return_the_correct_search_results_invariant()
  {
    SearchContentsPayload payload = new()
    {
      ContentTypeId = _blogArticleType.Id.ToGuid(),
      LanguageId = null,
      Ids = (await _contentRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("%acura%"), new SearchTerm("%red%")], SearchOperator.Or),
      Sort = [new ContentSortOption(ContentSort.UniqueName, isDescending: false)],
      Skip = 1,
      Limit = 1
    };

    payload.Ids.Remove(_blogArticle3.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchContentsQuery query = new(payload);
    SearchResults<ContentLocale> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, results.Total);
    Assert.Equal(payload.Limit, results.Items.Count);

    ContentLocale locale = Assert.Single(results.Items);
    Assert.Null(locale.Language);
    Assert.Equal(_blogArticle1.Id.ToGuid(), locale.Item.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (locale).")]
  public async Task It_should_return_the_correct_search_results_locale()
  {
    SearchContentsPayload payload = new()
    {
      ContentTypeId = _blogArticleType.Id.ToGuid(),
      LanguageId = _canadianFrench.Id.ToGuid(),
      Ids = (await _contentRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("%acura%"), new SearchTerm("%red%")], SearchOperator.Or),
      Sort = [new ContentSortOption(ContentSort.UniqueName, isDescending: false)],
      Skip = 1,
      Limit = 1
    };

    payload.Ids.Remove(_blogArticle3.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchContentsQuery query = new(payload);
    SearchResults<ContentLocale> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, results.Total);
    Assert.Equal(payload.Limit, results.Items.Count);

    ContentLocale locale = Assert.Single(results.Items);
    Assert.NotNull(locale.Language);
    Assert.Equal(_canadianFrench.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(_blogArticle1.Id.ToGuid(), locale.Item.Id);
  }
}
