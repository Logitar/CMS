using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Core.Shared;
using MediatR;

namespace Logitar.Cms.Core.Localization.Queries;

internal class ReadLanguageQueryHandler : IRequestHandler<ReadLanguageQuery, Language?>
{
  private readonly ILanguageQuerier _languageQuerier;

  public ReadLanguageQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<Language?> Handle(ReadLanguageQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Language> languages = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Language? language = await _languageQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (language != null)
      {
        languages[language.Id] = language;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Locale))
    {
      Language? language = await _languageQuerier.ReadAsync(query.Locale, cancellationToken);
      if (language != null)
      {
        languages[language.Id] = language;
      }
    }

    if (languages.Count > 1)
    {
      throw TooManyResultsException<Language>.ExpectedSingle(languages.Count);
    }

    return languages.SingleOrDefault().Value;
  }
}
