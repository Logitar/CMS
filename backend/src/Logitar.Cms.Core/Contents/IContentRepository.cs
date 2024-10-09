namespace Logitar.Cms.Core.Contents;

public interface IContentRepository
{
  Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Content content, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Content> contents, CancellationToken cancellationToken = default);
}
