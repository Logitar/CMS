using Logitar.Cms.Core.Localization.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Localization;

internal class LanguageManager : ILanguageManager
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public LanguageManager(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    Locale? locale = null;
    foreach (IEvent change in language.Changes)
    {
      if (change is LanguageCreated created)
      {
        locale = created.Locale;
      }
      else if (change is LanguageLocaleChanged localeChanged)
      {
        locale = localeChanged.Locale;
      }
    }

    if (locale != null)
    {
      LanguageId? conflictId = await _languageQuerier.FindIdAsync(locale, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(language.Id))
      {
        throw new LocaleAlreadyUsedException(language, conflictId.Value);
      }
    }

    await _languageRepository.SaveAsync(language, cancellationToken);
  }
}
