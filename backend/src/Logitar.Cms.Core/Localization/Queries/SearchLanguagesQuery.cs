﻿using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Search;
using MediatR;

namespace Logitar.Cms.Core.Localization.Queries;

public record SearchLanguagesQuery(SearchLanguagesPayload Payload) : IRequest<SearchResults<LanguageModel>>;

internal class SearchLanguagesQueryHandler : IRequestHandler<SearchLanguagesQuery, SearchResults<LanguageModel>>
{
  private readonly ILanguageQuerier _languageQuerier;

  public SearchLanguagesQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<SearchResults<LanguageModel>> Handle(SearchLanguagesQuery query, CancellationToken cancellationToken)
  {
    return await _languageQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
