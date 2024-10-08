﻿using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Languages.Queries;

public record SearchLanguagesQuery(SearchLanguagesPayload Payload) : Activity, IRequest<SearchResults<LanguageModel>>;

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
