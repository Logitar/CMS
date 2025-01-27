﻿using FluentValidation;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Localization.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

public record CreateOrReplaceLanguageResult(LanguageModel? Language = null, bool Created = false);

/// <exception cref="LocaleAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceLanguageCommand(Guid? Id, CreateOrReplaceLanguagePayload Payload, long? Version) : IRequest<CreateOrReplaceLanguageResult>;

internal class CreateOrReplaceLanguageCommandHandler : IRequestHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILanguageManager _languageManager;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public CreateOrReplaceLanguageCommandHandler(
    IApplicationContext applicationContext,
    ILanguageManager languageManager,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _languageManager = languageManager;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<CreateOrReplaceLanguageResult> Handle(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    new CreateOrReplaceLanguageValidator().ValidateAndThrow(payload);

    ActorId? actorId = _applicationContext.ActorId;
    Locale locale = new(payload.Locale);

    LanguageId? languageId = null;
    Language? language = null;
    if (command.Id.HasValue)
    {
      languageId = new(command.Id.Value);
      language = await _languageRepository.LoadAsync(languageId.Value, cancellationToken);
    }

    bool created = false;
    if (language == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceLanguageResult();
      }

      language = new(locale, isDefault: false, actorId, languageId);
      created = true;
    }
    else
    {
      Language reference = (command.Version.HasValue
        ? await _languageRepository.LoadAsync(language.Id, command.Version.Value, cancellationToken)
        : null) ?? language;

      if (reference.Locale != locale)
      {
        language.SetLocale(locale, actorId);
      }
    }

    await _languageManager.SaveAsync(language, cancellationToken);

    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(model, created);
  }
}
