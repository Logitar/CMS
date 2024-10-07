using FluentValidation;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Core.Languages.Validators;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

public record CreateOrReplaceLanguageResult(LanguageModel? Language = null, bool Created = false);

public record CreateOrReplaceLanguageCommand(Guid? Id, CreateOrReplaceLanguagePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceLanguageResult>;

public class CreateOrReplaceLanguageCommandHandler : IRequestHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public CreateOrReplaceLanguageCommandHandler(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository, ISender sender)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _sender = sender;
  }

  public async Task<CreateOrReplaceLanguageResult> Handle(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceLanguageValidator().ValidateAndThrow(command.Payload);

    bool created = false;
    Language? language = await FindAsync(command, cancellationToken);
    if (language == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceLanguageResult();
      }

      language = Create(command);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, language, cancellationToken);
    }

    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(model, created);
  }

  private async Task<Language?> FindAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    if (command.Id == null)
    {
      return null;
    }

    LanguageId languageId = new(command.Id.Value);
    return await _languageRepository.LoadAsync(languageId, cancellationToken);
  }

  private static Language Create(CreateOrReplaceLanguageCommand command)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    ActorId actorId = command.GetActorId();

    Locale locale = new(payload.Locale);
    LanguageId? languageId = command.Id.HasValue ? new(command.Id.Value) : null;
    Language language = new(locale, isDefault: false, actorId, languageId);

    return language;
  }

  private async Task ReplaceAsync(CreateOrReplaceLanguageCommand command, Language language, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    ActorId actorId = command.GetActorId();

    Language? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _languageRepository.LoadAsync(language.Id, command.Version.Value, cancellationToken);
    }
    reference ??= language;

    Locale locale = new(payload.Locale);
    if (!locale.Equals(reference.Locale))
    {
      language.Locale = locale;
    }

    language.Update(actorId);
  }
}
