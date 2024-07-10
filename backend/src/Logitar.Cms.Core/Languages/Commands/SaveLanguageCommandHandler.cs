using Logitar.Cms.Core.Languages.Events;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

internal class SaveLanguageCommandHandler : IRequestHandler<SaveLanguageCommand>
{
  private readonly ILanguageRepository _languageRepository;

  public SaveLanguageCommandHandler(ILanguageRepository languageRepository)
  {
    _languageRepository = languageRepository;
  }

  public async Task Handle(SaveLanguageCommand command, CancellationToken cancellationToken)
  {
    LanguageAggregate language = command.Language;

    bool hasLocaleChanged = false;
    foreach (DomainEvent change in language.Changes)
    {
      if (change is LanguageCreatedEvent)
      {
        hasLocaleChanged = true;
      }
    }

    if (hasLocaleChanged)
    {
      LanguageAggregate? other = await _languageRepository.LoadAsync(language.Locale, cancellationToken);
      if (other != null && !other.Equals(language))
      {
        throw new LocaleAlreadyUsedException(language.Locale, nameof(language.Locale));
      }
    }

    await _languageRepository.SaveAsync(language, cancellationToken);
  }
}
