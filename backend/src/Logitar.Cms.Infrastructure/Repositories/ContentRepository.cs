using Logitar.Cms.Core.Contents;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Repositories;

internal class ContentRepository : Repository, IContentRepository
{
  public ContentRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Content>(id.StreamId, version, cancellationToken);
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
