using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.Languages;

public interface ILanguageQuerier
{
  Task<Language> ReadAsync(LanguageAggregate language, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(string locale, CancellationToken cancellationToken = default);
  Task<Language> ReadDefaultAsync(CancellationToken cancellationToken = default);

  Task<SearchResults<Language>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
}
