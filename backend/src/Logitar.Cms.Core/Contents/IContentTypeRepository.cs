namespace Logitar.Cms.Core.Contents;

public interface IContentTypeRepository
{
  Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentType> contentTypes, CancellationToken cancellationToken = default);
}
