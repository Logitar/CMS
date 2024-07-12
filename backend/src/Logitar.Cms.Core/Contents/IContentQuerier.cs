using Logitar.Cms.Contracts.Contents;

namespace Logitar.Cms.Core.Contents;

public interface IContentQuerier
{
  Task<ContentItem> ReadAsync(ContentAggregate content, CancellationToken cancellationToken = default);
  Task<ContentItem?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentItem?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
