using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Search;
using Logitar.Identity.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class PublishedContentIntegrationTests : IntegrationTests
{
  private readonly IContentManager _contentManager;
  private readonly IContentTypeManager _contentTypeManager;
  private readonly ILanguageManager _languageManager;

  private readonly Language _french = new(new Locale("fr"));

  private readonly ContentType _vegetable = new(new Identifier("Vegetable"), isInvariant: false);
  private readonly Content _carrot;
  private readonly Content _celery;
  private readonly Content _potato;
  private readonly Content _tomato;

  public PublishedContentIntegrationTests()
  {
    _contentManager = ServiceProvider.GetRequiredService<IContentManager>();
    _contentTypeManager = ServiceProvider.GetRequiredService<IContentTypeManager>();
    _languageManager = ServiceProvider.GetRequiredService<ILanguageManager>();

    _carrot = new(_vegetable, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "carrot")));
    _carrot.SetLocale(_french, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "carotte")));
    _carrot.Publish();

    _celery = new(_vegetable, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "celery")));
    _celery.SetLocale(_french, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "celeri")));
    _celery.Publish();

    _potato = new(_vegetable, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "potato")));
    _potato.SetLocale(_french, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "patate")));
    _potato.Publish();

    _tomato = new(_vegetable, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "tomato")));
    _tomato.SetLocale(_french, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "tomate")));
    _tomato.Publish();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageManager.SaveAsync(_french);
    await _contentTypeManager.SaveAsync(_vegetable);
    await _contentManager.SaveAsync(_carrot);
    await _contentManager.SaveAsync(_celery);
    await _contentManager.SaveAsync(_potato);
    await _contentManager.SaveAsync(_tomato);
  }

  [Fact(DisplayName = "It should filter published contents by content ID (Guid).")]
  public async Task Given_GuidContentId_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    SearchPublishedContentsPayload payload = new()
    {
      Content = new()
      {
        Uids = [_potato.Id.ToGuid(), _tomato.Id.ToGuid(), Guid.NewGuid()]
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(4, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by content ID (Int32).")]
  public async Task Given_Int32ContentId_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    List<int> ids = await CmsContext.PublishedContents.AsNoTracking()
      .Where(x => x.UniqueName == "potato" || x.UniqueName == "tomato")
      .Select(x => x.ContentId)
      .Distinct()
      .ToListAsync();
    ids.Add(-1);

    SearchPublishedContentsPayload payload = new()
    {
      Content = new()
      {
        Ids = ids
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(4, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by content type ID (Guid).")]
  public async Task Given_GuidContentTypeId_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    SearchPublishedContentsPayload payload = new()
    {
      ContentType = new()
      {
        Uids = [_vegetable.Id.ToGuid(), Guid.NewGuid(), Guid.Empty]
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(8, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by content type ID (Int32).")]
  public async Task Given_Int32ContentTypeId_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    List<int> ids = await CmsContext.ContentTypes.AsNoTracking()
      .Where(x => x.UniqueName == "Vegetable")
      .Select(x => x.ContentTypeId)
      .Distinct()
      .ToListAsync();
    ids.Add(-1);

    SearchPublishedContentsPayload payload = new()
    {
      ContentType = new()
      {
        Ids = ids
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(8, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by content type name.")]
  public async Task Given_ContentTypeName_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    SearchPublishedContentsPayload payload = new()
    {
      ContentType = new()
      {
        Names = ["Vegetable", string.Empty, "    ", "Test"]
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(8, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by language code.")]
  public async Task Given_LanguageCode_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    SearchPublishedContentsPayload payload = new()
    {
      Language = new()
      {
        Codes = ["fr", "en", "es", string.Empty, "    ", "invalid"]
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(4, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by language ID (Guid).")]
  public async Task Given_GuidLanguageId_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    SearchPublishedContentsPayload payload = new()
    {
      Language = new()
      {
        Uids = [_french.Id.ToGuid(), Guid.NewGuid(), Guid.Empty]
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(4, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by language ID (Int32).")]
  public async Task Given_Int32LanguageId_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    List<int> ids = await CmsContext.Languages.AsNoTracking()
      .Where(x => x.Code == "fr")
      .Select(x => x.LanguageId)
      .Distinct()
      .ToListAsync();
    ids.Add(-1);

    SearchPublishedContentsPayload payload = new()
    {
      Language = new()
      {
        Ids = ids
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(4, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _potato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _tomato.Id.ToGuid() && locale.Language?.Id == _french.Id.ToGuid());
  }

  [Fact(DisplayName = "It should filter published contents by text search.")]
  public async Task Given_TextSearch_When_SearchAsync_Then_PublishedContentsFiltered()
  {
    SearchPublishedContentsPayload payload = new()
    {
      Search = new()
      {
        Operator = SearchOperator.Or,
        Terms = [new SearchTerm("%ot"), new SearchTerm("%y%")]
      }
    };
    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> publishedLocales = await Mediator.Send(query);

    Assert.Equal(2, publishedLocales.Total);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _carrot.Id.ToGuid() && locale.Language == null);
    Assert.Contains(publishedLocales.Items, locale => locale.Content.Id == _celery.Id.ToGuid() && locale.Language == null);
  }
}
