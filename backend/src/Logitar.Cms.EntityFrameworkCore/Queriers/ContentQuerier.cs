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
    ContentItemEntity? content = await _contentItems.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueId == id, cancellationToken);

    return content == null ? null : await MapAsync(content, cancellationToken);
  }

  private async Task<ContentItem> MapAsync(ContentItemEntity content, CancellationToken cancellationToken)
    => (await MapAsync([content], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<ContentItem>> MapAsync(IEnumerable<ContentItemEntity> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contents.Select(mapper.ToContentItem).ToArray();
  }
}
