using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Languages.Queries;

internal class SearchLanguagesQueryHandler : IRequestHandler<SearchLanguagesQuery, SearchResults<Language>>
{
  private readonly ILanguageQuerier _languageQuerier;

  public SearchLanguagesQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<SearchResults<Language>> Handle(SearchLanguagesQuery query, CancellationToken cancellationToken)
  {
    return await _languageQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
