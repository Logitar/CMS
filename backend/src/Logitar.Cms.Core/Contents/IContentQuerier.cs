using Logitar.Cms.Contracts.Contents;

namespace Logitar.Cms.Core.Contents;

public interface IContentQuerier
{
  Task<ContentModel> ReadAsync(Content content, CancellationToken cancellationToken = default);
  Task<ContentModel?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
