using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class ContentQuerier : IContentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentEntity> _contents;
  private readonly DbSet<ContentLocaleEntity> _contentLocales;
  private readonly IQueryHelper _queryHelper;

  public ContentQuerier(IActorService actorService, CmsContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _contents = context.Contents;
    _contentLocales = context.ContentLocales;
    _queryHelper = queryHelper;
  }

  public async Task<IReadOnlyDictionary<Guid, ContentId>> FindConflictsAsync(
    ContentTypeId contentTypeId,
    LanguageId? languageId,
    IReadOnlyDictionary<Guid, string> fieldValues,
    ContentId contentId,
    CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public async Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string contentTypeStreamId = contentTypeId.Value;
    string? languageStreamId = languageId?.Value;
    string uniqueNameNormalized = Helper.Normalize(uniqueName.Value);

    string? streamId = await _contentLocales.AsNoTracking()
      .Include(x => x.Content)
      .Include(x => x.ContentType)
      .Include(x => x.Language)
      .Where(x => x.ContentType!.StreamId == contentTypeStreamId
        && (languageStreamId == null ? x.Language!.StreamId == null : x.Language!.StreamId == languageStreamId)
        && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.Content!.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new ContentId(streamId);
  }

  public async Task<ContentModel> ReadAsync(Content content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The content entity 'StreamId={content.Id}' could not be found.");
  }
  public async Task<ContentModel?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ContentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _contents.AsNoTracking()
      .Include(x => x.ContentType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return content == null ? null : await MapAsync(content, cancellationToken);
  }

  public async Task<SearchResults<ContentLocaleModel>> SearchAsync(SearchContentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(CmsDb.ContentLocales.Table).SelectAll(CmsDb.ContentLocales.Table)
      .Join(CmsDb.Contents.ContentId, CmsDb.ContentLocales.ContentId)
      .ApplyIdFilter(CmsDb.Contents.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, CmsDb.ContentLocales.UniqueName, CmsDb.ContentLocales.DisplayName);

    if (payload.ContentTypeId.HasValue)
    {
      builder.Join(CmsDb.ContentTypes.ContentTypeId, CmsDb.ContentLocales.ContentTypeId)
        .Where(CmsDb.ContentTypes.Id, Operators.IsEqualTo(payload.ContentTypeId.Value.ToString()));
    }

    if (payload.LanguageId.HasValue)
    {
      builder.Join(CmsDb.Languages.LanguageId, CmsDb.ContentLocales.LanguageId)
        .Where(CmsDb.Languages.Id, Operators.IsEqualTo(payload.LanguageId.Value.ToString()));
    }
    else
    {
      builder.Where(CmsDb.ContentLocales.LanguageId, Operators.IsNull());
    }

    IQueryable<ContentLocaleEntity> query = _contentLocales.FromQuery(builder).AsNoTracking()
      .Include(x => x.Content).ThenInclude(x => x!.ContentType)
      .Include(x => x.Language);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ContentLocaleEntity>? ordered = null;
    foreach (ContentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ContentSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ContentSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ContentSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case ContentSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ContentLocaleEntity[] contentLocales = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ContentLocaleModel> items = await MapAsync(contentLocales, cancellationToken);

    return new SearchResults<ContentLocaleModel>(items, total);
  }

  private async Task<ContentModel> MapAsync(ContentEntity content, CancellationToken cancellationToken)
  {
    return (await MapAsync([content], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentModel>> MapAsync(IEnumerable<ContentEntity> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contents.Select(mapper.ToContent).ToArray();
  }

  private async Task<ContentLocaleModel> MapAsync(ContentLocaleEntity locale, CancellationToken cancellationToken)
  {
    return (await MapAsync([locale], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentLocaleModel>> MapAsync(IEnumerable<ContentLocaleEntity> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return locales.Select(mapper.ToContentLocale).ToArray();
  }
}
