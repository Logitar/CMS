using Logitar.Cms.Core.Localization.Events;
using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

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

    if (language.Changes.Any(change => change is LanguageCreatedEvent))
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
