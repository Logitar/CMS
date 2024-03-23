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

  public async Task<ContentType> ReadAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken)
  {
    return await ReadAsync(contentType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The content type entity 'AggregateId={contentType.Id.AggregateId}' could not be found.");
  }
  public async Task<ContentType?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ContentType?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  public async Task<ContentType?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  private async Task<ContentType> MapAsync(ContentTypeEntity contentType, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contentType.GetActorIds();
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToContentType(contentType);
  }
}
