using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Core.ContentTypes;

public interface IContentTypeQuerier
{
  Task<ContentType> ReadAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken = default);
  Task<ContentType?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentType?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ContentType?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
