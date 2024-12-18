using Logitar.Cms.Core.Localization.Models;

namespace Logitar.Cms.Core.Localization;

public interface ILanguageQuerier
{
  Task<LanguageId> FindDefaultIdAsync(CancellationToken cancellationToken = default);
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(string locale, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken = default);
}
