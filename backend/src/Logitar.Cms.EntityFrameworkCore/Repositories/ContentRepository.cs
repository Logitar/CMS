using Logitar.Cms.Core.Contents;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ContentRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IContentRepository
{
  public ContentRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Content>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    await base.SaveAsync(content, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Content> contents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contents, cancellationToken);
  }
}
