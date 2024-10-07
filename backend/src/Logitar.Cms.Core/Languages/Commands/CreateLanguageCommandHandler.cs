using FluentValidation;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Core.Languages.Validators;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

internal class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, Language>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ISender _sender;

  public CreateLanguageCommandHandler(ILanguageQuerier languageQuerier, ISender sender)
  {
    _languageQuerier = languageQuerier;
    _sender = sender;
  }

  public async Task<Language> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateLanguagePayload payload = command.Payload;
    new CreateLanguageValidator().ValidateAndThrow(payload);

    LanguageAggregate language = new(new LocaleUnit(payload.Locale), isDefault: false, command.ActorId);

    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
