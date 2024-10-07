using Logitar.Cms.Contracts.Languages;

namespace Logitar.Cms.Core.Languages;

public interface ILanguageQuerier
{
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(string locale, CancellationToken cancellationToken = default);
  Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken = default);
}
