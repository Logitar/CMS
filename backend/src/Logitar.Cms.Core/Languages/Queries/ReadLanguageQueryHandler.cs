using Logitar.Cms.Contracts.Languages;
using MediatR;

namespace Logitar.Cms.Core.Languages.Queries;

internal class ReadLanguageQueryHandler : IRequestHandler<ReadLanguageQuery, Language?>
{
  private readonly ILanguageQuerier _languageQuerier;

  public ReadLanguageQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<Language?> Handle(ReadLanguageQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Language> languages = new(capacity: 3);

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

    if (query.IsDefault)
    {
      Language language = await _languageQuerier.ReadDefaultAsync(cancellationToken);
      languages[language.Id] = language;
    }

    if (languages.Count > 1)
    {
      throw TooManyResultsException<Language>.ExpectedSingle(languages.Count);
    }

    return languages.Values.SingleOrDefault();
  }
}
