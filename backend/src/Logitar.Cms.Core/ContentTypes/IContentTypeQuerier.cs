using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.ContentTypes;

public interface IContentTypeQuerier
{
  Task<CmsContentType> ReadAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken = default);
  Task<CmsContentType?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<CmsContentType?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<CmsContentType?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<CmsContentType>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken = default);
}
