using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

internal class SearchContentTypesQueryHandler : IRequestHandler<SearchContentTypesQuery, SearchResults<ContentsType>>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public SearchContentTypesQueryHandler(IContentTypeQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<SearchResults<ContentsType>> Handle(SearchContentTypesQuery query, CancellationToken cancellationToken)
  {
    return await _contentTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
