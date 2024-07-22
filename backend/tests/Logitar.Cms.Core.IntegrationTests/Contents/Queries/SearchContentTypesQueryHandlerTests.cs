using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchContentsQueryHandlerTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly ContentTypeAggregate _articleType;
  private readonly ContentTypeAggregate _productType;

  private readonly ContentAggregate _integraTypeS;
  private readonly ContentAggregate _renderedLegoModels;
  private readonly ContentAggregate _electricVisionDesign;
  private readonly ContentAggregate _integraFrontSplitter;
  private readonly ContentAggregate _customGrillesTlx;
  private readonly ContentAggregate _tlxTypeS;

  public SearchContentsQueryHandlerTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _articleType = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _productType = new(new IdentifierUnit("Product"), isInvariant: false);

    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;
    _integraTypeS = new(_articleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "video-integra-type-s-makes-605-whp-on-stock-internals")));
    _renderedLegoModels = new(_articleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "rendered-lego-acura-models")));
    _electricVisionDesign = new(_articleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "acura-previews-performance-electric-vision-design-study-at-monterey-car-week")));
    _integraFrontSplitter = new(_articleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "gallery-2023-acura-integra-front-splitter-options")));
    _customGrillesTlx = new(_articleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "custom-grilles-for-the-2015-2017-acura-tlx")));
    _tlxTypeS = new(_productType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "acura-tlx-type-s")));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync([_articleType, _productType]);
    await _contentRepository.SaveAsync([_integraTypeS, _renderedLegoModels, _electricVisionDesign, _integraFrontSplitter, _customGrillesTlx, _tlxTypeS]);
  }

  [Fact(DisplayName = "It should return empty results when no content matches.")]
  public async Task It_should_return_empty_results_when_no_content_matches()
  {
    SearchContentsPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };
    SearchContentsQuery query = new(payload);

    SearchResults<ContentLocale> results = await Pipeline.ExecuteAsync(query);

    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct matching contents.")]
  public async Task It_should_return_the_correct_matching_contents()
  {
    LanguageAggregate english = await _languageRepository.LoadDefaultAsync();
    _integraTypeS.SetLocale(english, _integraTypeS.Invariant);
    _renderedLegoModels.SetLocale(english, _renderedLegoModels.Invariant);
    _electricVisionDesign.SetLocale(english, _electricVisionDesign.Invariant);
    _customGrillesTlx.SetLocale(english, _customGrillesTlx.Invariant);
    _tlxTypeS.SetLocale(english, _tlxTypeS.Invariant);
    await _contentRepository.SaveAsync([_integraTypeS, _renderedLegoModels, _electricVisionDesign, _customGrillesTlx, _tlxTypeS]);

    SearchContentsPayload payload = new()
    {
      ContentTypeId = _articleType.Id.ToGuid(),
      LanguageId = english.Id.ToGuid(),
      IdIn = (await _contentRepository.LoadAsync()).Select(content => content.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("%acura%")]),
      Sort = [new ContentSortOption(ContentSort.UniqueName, isDescending: false)],
      Skip = 1,
      Limit = 1
    };
    payload.IdIn.Add(Guid.Empty);
    payload.IdIn.Remove(_electricVisionDesign.Id.ToGuid());
    SearchContentsQuery query = new(payload);

    SearchResults<ContentLocale> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    ContentLocale locale = Assert.Single(results.Items);
    Assert.Equal(_renderedLegoModels.Id.ToGuid(), locale.Item.Id);
  }
}
