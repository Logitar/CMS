using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class PublishedContentQuerier : IPublishedContentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<LanguageEntity> _languages;
  private readonly DbSet<PublishedContentEntity> _publishedContents;

  public PublishedContentQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _languages = context.Languages;
    _publishedContents = context.PublishedContents;
  }

  public async Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] locales = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentId == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapAsync(locales, cancellationToken)).Single();
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] locales = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentUid == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapAsync(locales, cancellationToken)).Single();
  }

  public async Task<PublishedContent?> ReadAsync(int contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(string contentTypeName, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(int contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(string contentTypeName, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(int contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    if (languageCode != null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    if (languageCode != null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(string contentTypeName, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    if (languageCode != null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }

  private async Task<IReadOnlyCollection<PublishedContent>> MapAsync(IEnumerable<PublishedContentEntity> locales, CancellationToken cancellationToken)
  {
    int defaultLanguageId = (await _languages.AsNoTracking()
      .Where(x => x.IsDefault)
      .Select(x => (int?)x.LanguageId)
      .SingleOrDefaultAsync(cancellationToken)) ?? throw new InvalidOperationException("The default language entity could not be found.");

    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Dictionary<int, ContentTypeSummary> contentTypes = [];
    Dictionary<int, LanguageSummary> languages = [];
    Dictionary<int, PublishedContent> publishedContents = [];
    foreach (PublishedContentEntity locale in locales)
    {
      if (!contentTypes.TryGetValue(locale.ContentTypeId, out ContentTypeSummary? contentType))
      {
        contentType = new(locale.ContentTypeUid, locale.ContentTypeName);
        contentTypes[locale.ContentTypeId] = contentType;
      }

      if (!publishedContents.TryGetValue(locale.ContentId, out PublishedContent? publishedContent))
      {
        publishedContent = new()
        {
          Id = locale.ContentUid,
          ContentType = contentType
        };
        publishedContents[locale.ContentId] = publishedContent;
      }

      LanguageSummary? language = null;
      if (locale.LanguageId.HasValue && !languages.TryGetValue(locale.LanguageId.Value, out language))
      {
        language = new()
        {
          IsDefault = locale.LanguageId.Value == defaultLanguageId
        };
        if (locale.LanguageUid.HasValue)
        {
          language.Id = locale.LanguageUid.Value;
        }
        if (locale.LanguageCode != null)
        {
          language.Locale = new(locale.LanguageCode);
        }
        languages[locale.LanguageId.Value] = language;
      }

      PublishedContentLocale publishedLocale = mapper.ToPublishedContentLocale(locale, publishedContent, language);
      if (language == null)
      {
        publishedContent.Invariant = publishedLocale;
      }
      else
      {
        publishedContent.Locales.Add(publishedLocale);
      }
    }

    return publishedContents.Values;
  }
}
