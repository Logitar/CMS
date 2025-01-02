using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class ContentTypeQuerier : IContentTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentTypeEntity> _contentTypes;
  private readonly IQueryHelper _queryHelper;

  public ContentTypeQuerier(IActorService actorService, CmsContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _contentTypes = context.ContentTypes;
    _queryHelper = queryHelper;
  }

  public async Task<ContentTypeId?> FindIdAsync(Identifier uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName.Value);

    string? streamId = await _contentTypes.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new ContentTypeId(streamId);
  }

  public async Task<ContentTypeModel> ReadAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    return await ReadAsync(contentType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The content type entity 'StreamId={contentType.Id}' could not be found.");
  }
  public async Task<ContentTypeModel?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ContentTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }
  public async Task<ContentTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  public async Task<SearchResults<ContentTypeModel>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(CmsDb.ContentTypes.Table).SelectAll(CmsDb.ContentTypes.Table)
      .ApplyIdFilter(CmsDb.ContentTypes.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, CmsDb.ContentTypes.UniqueName, CmsDb.ContentTypes.DisplayName);

    if (payload.IsInvariant.HasValue)
    {
      builder.Where(CmsDb.ContentTypes.IsInvariant, Operators.IsEqualTo(payload.IsInvariant.Value));
    }

    IQueryable<ContentTypeEntity> query = _contentTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ContentTypeEntity>? ordered = null;
    foreach (ContentTypeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ContentTypeSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
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
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ContentTypeEntity[] contentTypes = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ContentTypeModel> items = await MapAsync(contentTypes, cancellationToken);

    return new SearchResults<ContentTypeModel>(items, total);
  }

  private async Task<ContentTypeModel> MapAsync(ContentTypeEntity contentType, CancellationToken cancellationToken)
  {
    return (await MapAsync([contentType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentTypeModel>> MapAsync(IEnumerable<ContentTypeEntity> contentTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contentTypes.SelectMany(contentType => contentType.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contentTypes.Select(mapper.ToContentType).ToArray();
  }
}
