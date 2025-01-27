﻿using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Search;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

public record SearchContentTypesQuery(SearchContentTypesPayload Payload) : IRequest<SearchResults<ContentTypeModel>>;

internal class SearchContentTypesQueryHandler : IRequestHandler<SearchContentTypesQuery, SearchResults<ContentTypeModel>>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public SearchContentTypesQueryHandler(IContentTypeQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<SearchResults<ContentTypeModel>> Handle(SearchContentTypesQuery query, CancellationToken cancellationToken)
  {
    return await _contentTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
