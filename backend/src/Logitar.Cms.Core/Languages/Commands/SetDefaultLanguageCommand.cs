using Logitar.Cms.Contracts.Languages;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

public record SetDefaultLanguageCommand(Guid Id) : Activity, IRequest<LanguageModel?>;

internal class SetDefaultLanguageCommandHandler : IRequestHandler<SetDefaultLanguageCommand, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public SetDefaultLanguageCommandHandler(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<LanguageModel?> Handle(SetDefaultLanguageCommand command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(command.Id);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language == null)
    {
      return null;
    }
    else if (!language.IsDefault)
    {
      ActorId actorId = command.GetActorId();

      Language @default = await _languageRepository.LoadDefaultAsync(cancellationToken);
      @default.SetDefault(isDefault: false, actorId);

      language.SetDefault(actorId);

      await _languageRepository.SaveAsync([@default, language], cancellationToken);
    }

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
