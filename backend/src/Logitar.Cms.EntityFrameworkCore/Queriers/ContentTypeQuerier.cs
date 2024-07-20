using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ContentTypeQuerier : IContentTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentTypeEntity> _contentTypes;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ContentTypeQuerier(IActorService actorService, CmsContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _contentTypes = context.ContentTypes;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<CmsContentType> ReadAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken)
  {
    return await ReadAsync(contentType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The content type entity 'AggregateId={contentType.Id.AggregateId}' could not be found.");
  }
  public async Task<CmsContentType?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<CmsContentType?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .Include(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
      .SingleOrDefaultAsync(x => x.UniqueId == id, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  public async Task<CmsContentType?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = CmsDb.Normalize(uniqueName);

    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .Include(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  public async Task<SearchResults<CmsContentType>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.ContentTypes.Table).SelectAll(CmsDb.ContentTypes.Table)
      .ApplyIdInFilter(CmsDb.ContentTypes.UniqueId, payload);
    _searchHelper.ApplyTextSearch(builder, payload.Search, CmsDb.ContentTypes.UniqueName, CmsDb.ContentTypes.DisplayName);

    if (payload.IsInvariant.HasValue)
    {
      builder.Where(CmsDb.ContentTypes.IsInvariant, Operators.IsEqualTo(payload.IsInvariant.Value));
    }

    IQueryable<ContentTypeEntity> query = _contentTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ContentTypeEntity>? ordered = null;
    if (payload.Sort != null)
    {
      foreach (ContentTypeSortOption sort in payload.Sort)
      {
        switch (sort.Field)
        {
          case ContentTypeSort.DisplayName:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
            break;
          case ContentTypeSort.UniqueName:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
            break;
          case ContentTypeSort.UpdatedOn:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
            break;
        }
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ContentTypeEntity[] fieldTypes = await query.ToArrayAsync(cancellationToken);
    IEnumerable<CmsContentType> items = await MapAsync(fieldTypes, cancellationToken);

    return new SearchResults<CmsContentType>(items, total);
  }

  private async Task<CmsContentType> MapAsync(ContentTypeEntity contentType, CancellationToken cancellationToken)
    => (await MapAsync([contentType], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<CmsContentType>> MapAsync(IEnumerable<ContentTypeEntity> contentTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contentTypes.SelectMany(contentType => contentType.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contentTypes.Select(mapper.ToContentType).ToArray();
  }
}
