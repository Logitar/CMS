using FluentValidation;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Core.Languages.Validators;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

public record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : Activity, IRequest<LanguageModel?>;

public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public UpdateLanguageCommandHandler(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository, ISender sender)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _sender = sender;
  }

  public async Task<LanguageModel?> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    new UpdateLanguageValidator().ValidateAndThrow(command.Payload);

    LanguageId languageId = new(command.Id);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language == null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.Locale))
    {
      language.Locale = new Locale(payload.Locale);
    }

    ActorId actorId = command.GetActorId();
    language.Update(actorId);

    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
