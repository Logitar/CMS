using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.Languages;

public interface ILanguageQuerier
{
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(string locale, CancellationToken cancellationToken = default);
  Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken = default);

  Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
}
