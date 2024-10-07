using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.Contents;

public interface IContentQuerier
{
  Task<ContentItem> ReadAsync(ContentAggregate content, CancellationToken cancellationToken = default);
  Task<ContentItem?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentItem?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ContentLocale>> SearchAsync(SearchContentsPayload payload, CancellationToken cancellationToken = default);
}
