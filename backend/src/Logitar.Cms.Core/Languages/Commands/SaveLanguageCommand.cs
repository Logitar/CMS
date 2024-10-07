using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

internal record SaveLanguageCommand(Language Language) : IRequest;

internal class SaveLanguageCommandHandler : IRequestHandler<SaveLanguageCommand>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public SaveLanguageCommandHandler(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task Handle(SaveLanguageCommand command, CancellationToken cancellationToken)
  {
    Language language = command.Language;

    bool hasLocaleChanged = false;
    foreach (DomainEvent change in language.Changes)
    {
      if (change is Language.CreatedEvent || change is Language.UpdatedEvent updatedEvent && updatedEvent.Locale != null)
      {
        hasLocaleChanged = true;
      }
    }

    if (hasLocaleChanged)
    {
      LanguageId? conflictId = await _languageQuerier.FindIdAsync(language.Locale, cancellationToken);
      if (conflictId.HasValue && conflictId.Value != language.Id)
      {
        throw new LocaleAlreadyUsedException(language, conflictId.Value);
      }
    }

    await _languageRepository.SaveAsync(language, cancellationToken);
  }
}
