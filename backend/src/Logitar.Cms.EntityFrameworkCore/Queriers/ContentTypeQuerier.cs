using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ContentTypeQuerier : IContentTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentTypeEntity> _contentTypes;

  public ContentTypeQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _contentTypes = context.ContentTypes;
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
      .SingleOrDefaultAsync(x => x.UniqueId == id, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  public async Task<CmsContentType?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = CmsDb.Normalize(uniqueName);

    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
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
