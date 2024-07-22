using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

internal class SearchContentsQueryHandler : IRequestHandler<SearchContentsQuery, SearchResults<ContentLocale>>
{
  private readonly IContentQuerier _contentQuerier;

  public SearchContentsQueryHandler(IContentQuerier fieldTypeQuerier)
  {
    _contentQuerier = fieldTypeQuerier;
  }

  public async Task<SearchResults<ContentLocale>> Handle(SearchContentsQuery query, CancellationToken cancellationToken)
  {
    return await _contentQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
