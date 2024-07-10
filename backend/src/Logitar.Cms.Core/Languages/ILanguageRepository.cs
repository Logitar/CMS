using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Languages;

public interface ILanguageRepository
{
  Task<LanguageAggregate?> LoadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageAggregate?> LoadAsync(LocaleUnit locale, CancellationToken cancellationToken = default);
  Task<LanguageAggregate> LoadDefaultAsync(CancellationToken cancellationToken = default);

  Task SaveAsync(LanguageAggregate language, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<LanguageAggregate> languages, CancellationToken cancellationToken = default);
}
