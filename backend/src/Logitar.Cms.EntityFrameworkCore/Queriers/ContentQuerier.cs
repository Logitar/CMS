using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ContentQuerier : IContentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentItemEntity> _contentItems;
  private readonly DbSet<ContentLocaleEntity> _contentLocales;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ContentQuerier(IActorService actorService, CmsContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _contentItems = context.ContentItems;
    _contentLocales = context.ContentLocales;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<ContentItem> ReadAsync(ContentAggregate content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The content item entity (AggregateId={content.Id.AggregateId}) could not be found.");
  }
  public async Task<ContentItem?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ContentItem?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ContentItemEntity? content = await _contentItems.AsNoTracking()
      .Include(x => x.ContentType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return content == null ? null : await MapAsync(content, cancellationToken);
  }

  public async Task<SearchResults<ContentLocale>> SearchAsync(SearchContentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.ContentLocales.Table).SelectAll(CmsDb.ContentLocales.Table)
      .Join(CmsDb.ContentItems.ContentItemId, CmsDb.ContentLocales.ContentItemId)
      .Join(CmsDb.ContentTypes.ContentTypeId, CmsDb.ContentItems.ContentTypeId)
      .LeftJoin(CmsDb.Languages.LanguageId, CmsDb.ContentLocales.LanguageId)
      .ApplyIdInFilter(CmsDb.ContentItems.AggregateId, payload);
    _searchHelper.ApplyTextSearch(builder, payload.Search, CmsDb.ContentLocales.UniqueName);

    if (payload.ContentTypeId.HasValue)
    {
      ContentTypeId contentTypeId = new(payload.ContentTypeId.Value);
      builder.Where(CmsDb.ContentTypes.AggregateId, Operators.IsEqualTo(contentTypeId.Value));
    }

    LanguageId? languageId = payload.LanguageId.HasValue ? new(payload.LanguageId.Value) : null;
    builder.Where(CmsDb.Languages.AggregateId, languageId.HasValue ? Operators.IsEqualTo(languageId.Value.Value) : Operators.IsNull());

    IQueryable<ContentLocaleEntity> query = _contentLocales.FromQuery(builder).AsNoTracking()
      .Include(x => x.ContentItem).ThenInclude(x => x!.ContentType)
      .Include(x => x.Language);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ContentLocaleEntity>? ordered = null;
    if (payload.Sort != null)
    {
      foreach (ContentSortOption sort in payload.Sort)
      {
        switch (sort.Field)
        {
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
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ContentLocaleEntity[] locales = await query.ToArrayAsync(cancellationToken);
    IEnumerable<ContentLocale> items = await MapAsync(locales, cancellationToken);

    return new SearchResults<ContentLocale>(items, total);
  }

  private async Task<ContentItem> MapAsync(ContentItemEntity content, CancellationToken cancellationToken)
  {
    return (await MapAsync([content], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentItem>> MapAsync(IEnumerable<ContentItemEntity> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contents.Select(mapper.ToContentItem).ToArray();
  }

  private async Task<IReadOnlyCollection<ContentLocale>> MapAsync(IEnumerable<ContentLocaleEntity> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds(includeItem: true));
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return locales.Select(mapper.ToContentLocale).ToArray();
  }
}
