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
  private readonly DbSet<ContentEntity> _contents;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ContentQuerier(IActorService actorService, CmsContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _contents = context.Contents;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<ContentModel> ReadAsync(Content content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The content entity 'AggregateId={content.Id}' could not be found.");
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

  private async Task<ContentModel> MapAsync(ContentEntity content, CancellationToken cancellationToken)
  {
    return (await MapAsync([content], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentModel>> MapAsync(IEnumerable<ContentEntity> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contents.Select(mapper.ToContent).ToArray().AsReadOnly();
  }
}
