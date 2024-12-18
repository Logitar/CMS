using FluentValidation;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Localization.Validators;
using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

public record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : IRequest<LanguageModel?>;

internal class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMediator _mediator;

  public UpdateLanguageCommandHandler(
    IApplicationContext applicationContext,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _mediator = mediator;
  }

  public async Task<LanguageModel?> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    new UpdateLanguageValidator().ValidateAndThrow(payload);

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

    language.Update(_applicationContext.ActorId);

    await _mediator.Send(new SaveLanguageCommand(language), cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
