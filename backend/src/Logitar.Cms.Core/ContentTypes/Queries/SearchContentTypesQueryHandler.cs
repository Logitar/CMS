using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

internal class SearchContentTypesQueryHandler : IRequestHandler<SearchContentTypesQuery, SearchResults<CmsContentType>>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public SearchContentTypesQueryHandler(IContentTypeQuerier fieldTypeQuerier)
  {
    _contentTypeQuerier = fieldTypeQuerier;
  }

  public async Task<SearchResults<CmsContentType>> Handle(SearchContentTypesQuery query, CancellationToken cancellationToken)
  {
    return await _contentTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
