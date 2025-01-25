using Logitar.Cms.Core.Contents.Models;

namespace Logitar.Cms.Core.Contents;

public interface IPublishedContentQuerier
{
  Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken = default);
}
