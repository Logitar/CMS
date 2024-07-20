using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

internal class SearchContentTypesQueryHandler : IRequestHandler<SearchContentTypesQuery, SearchResults<CmsContentType>>
{
  private readonly IContentTypeQuerier _fieldTypeQuerier;

  public SearchContentTypesQueryHandler(IContentTypeQuerier fieldTypeQuerier)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
  }

  public async Task<SearchResults<CmsContentType>> Handle(SearchContentTypesQuery query, CancellationToken cancellationToken)
  {
    return await _fieldTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
