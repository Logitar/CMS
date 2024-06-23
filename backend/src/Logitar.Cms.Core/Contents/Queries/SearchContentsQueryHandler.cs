using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

internal class SearchContentsQueryHandler : IRequestHandler<SearchContentsQuery, SearchResults<ContentLocale>>
{
  private readonly IContentQuerier _contentTypeQuerier;

  public SearchContentsQueryHandler(IContentQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<SearchResults<ContentLocale>> Handle(SearchContentsQuery query, CancellationToken cancellationToken)
  {
    return await _contentTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
