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

  public async Task SaveAsync(ContentAggregate content, CancellationToken cancellationToken)
  {
    await base.SaveAsync(content, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ContentAggregate> contents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contents, cancellationToken);
  }
}
