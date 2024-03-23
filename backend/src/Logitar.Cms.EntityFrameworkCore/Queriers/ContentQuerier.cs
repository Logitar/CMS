using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ContentQuerier : IContentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentItemEntity> _contentItems;

  public ContentQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _contentItems = context.ContentItems;
  }

  public async Task<ContentItem> ReadAsync(ContentAggregate content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The content item entity 'AggregateId={content.Id.AggregateId}' could not be found.");
  }
  public async Task<ContentItem?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ContentItem?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ContentItemEntity? contentItem = await _contentItems.AsNoTracking()
      .Include(x => x.ContentType)
      .Include(x => x.ContentLocales)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return contentItem == null ? null : await MapAsync(contentItem, cancellationToken);
  }

  private async Task<ContentItem> MapAsync(ContentItemEntity contentItem, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contentItem.GetActorIds();
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToContentItem(contentItem);
  }
}
