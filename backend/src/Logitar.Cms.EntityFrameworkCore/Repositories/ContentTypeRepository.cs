using Logitar.Cms.Core.ContentTypes;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ContentTypeRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IContentTypeRepository
{
  public ContentTypeRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<ContentType>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ContentType> contentTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentTypes, cancellationToken);
  }
}
