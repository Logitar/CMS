using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.ContentTypes;

public interface IContentTypeQuerier
{
  Task<ContentsType> ReadAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken = default);
  Task<ContentsType?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentsType?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ContentsType?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<ContentsType>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken = default);
}
