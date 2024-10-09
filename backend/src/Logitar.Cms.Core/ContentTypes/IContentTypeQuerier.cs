using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.ContentTypes;

public interface IContentTypeQuerier
{
  Task<ContentTypeId?> FindIdAsync(Identifier uniqueName, CancellationToken cancellationToken = default);

  Task<ContentTypeModel> ReadAsync(ContentType contentType, CancellationToken cancellationToken = default);
  Task<ContentTypeModel?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ContentTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<ContentTypeModel>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken = default);
}
