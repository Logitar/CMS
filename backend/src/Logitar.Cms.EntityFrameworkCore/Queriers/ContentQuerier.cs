using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ContentQuerier : IContentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ContentItemEntity> _contentItems;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ContentQuerier(IActorService actorService, CmsContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _contentItems = context.ContentItems;
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
}
