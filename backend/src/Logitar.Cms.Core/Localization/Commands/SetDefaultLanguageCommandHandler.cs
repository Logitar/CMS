using Logitar.Cms.Contracts.Localization;
using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

internal class SetDefaultLanguageCommandHandler : IRequestHandler<SetDefaultLanguageCommand, Language?>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public SetDefaultLanguageCommandHandler(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<Language?> Handle(SetDefaultLanguageCommand command, CancellationToken cancellationToken)
  {
    LanguageId id = new(command.Id);
    LanguageAggregate? language = await _languageRepository.LoadAsync(id, cancellationToken);
    if (language == null)
    {
      return null;
    }

    LanguageAggregate @default = await _languageRepository.LoadDefaultAsync(cancellationToken);
    if (@default != language)
    {
      @default.SetDefault(false, command.ActorId);
      language.SetDefault(true, command.ActorId);

      await _languageRepository.SaveAsync([@default, language], cancellationToken);
    }

    return await _languageQuerier.ReadDefaultAsync(cancellationToken);
  }
}
