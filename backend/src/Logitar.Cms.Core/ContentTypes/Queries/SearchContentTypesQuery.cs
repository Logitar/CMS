using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

public record SearchContentTypesQuery(SearchContentTypesPayload Payload) : IRequest<SearchResults<CmsContentType>>;
