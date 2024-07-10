using Logitar.Cms.Contracts.Languages;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

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
    LanguageId languageId = new(command.Id);
    LanguageAggregate? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language == null)
    {
      return null;
    }

    if (!language.IsDefault)
    {
      LanguageAggregate @default = await _languageRepository.LoadDefaultAsync(cancellationToken);
      @default.SetDefault(isDefault: false, command.ActorId);

      language.SetDefault(isDefault: true, command.ActorId);

      await _languageRepository.SaveAsync([@default, language], cancellationToken);
    }

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
