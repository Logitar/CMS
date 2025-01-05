using Logitar.Cms.Core.Contents;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Repositories;

internal class ContentTypeRepository : Repository, IContentTypeRepository
{
  public ContentTypeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<ContentType>(id.StreamId, version, cancellationToken);
  }

  public async Task<ContentType> LoadAsync(Content content, CancellationToken cancellationToken)
  {
    return await LoadAsync(content.ContentTypeId, cancellationToken)
      ?? throw new InvalidOperationException($"The content type 'Id={content.ContentTypeId}' could not be loaded.");
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
