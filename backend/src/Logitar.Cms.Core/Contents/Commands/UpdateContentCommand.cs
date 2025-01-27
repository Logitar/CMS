﻿using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

/// <exception cref="ContentFieldValueConflictException"></exception>
/// <exception cref="ContentUniqueNameAlreadyUsedException"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateContentCommand(Guid ContentId, Guid? LanguageId, UpdateContentPayload Payload) : IRequest<ContentModel?>;

internal class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentManager _contentManager;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly ILanguageRepository _languageRepository;

  public UpdateContentCommandHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _contentManager = contentManager;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _languageRepository = languageRepository;
  }

  public async Task<ContentModel?> Handle(UpdateContentCommand command, CancellationToken cancellationToken)
  {
    UpdateContentPayload payload = command.Payload;
    new UpdateContentValidator().ValidateAndThrow(payload);

    ContentId contentId = new(command.ContentId);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    ContentLocale? invariantOrLocale;
    Language? language = null;
    if (command.LanguageId.HasValue)
    {
      LanguageId languageId = new(command.LanguageId.Value);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, nameof(command.LanguageId));

      invariantOrLocale = content.TryGetLocale(language);
      if (invariantOrLocale == null)
      {
        return null;
      }
    }
    else
    {
      invariantOrLocale = content.Invariant;
    }

    UniqueName uniqueName = string.IsNullOrWhiteSpace(payload.UniqueName) ? invariantOrLocale.UniqueName : new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = payload.DisplayName == null ? invariantOrLocale.DisplayName : DisplayName.TryCreate(payload.DisplayName.Value);
    Description? description = payload.Description == null ? invariantOrLocale.Description : Description.TryCreate(payload.Description.Value);

    Dictionary<Guid, string> fieldValues = new(invariantOrLocale.FieldValues);
    foreach (FieldValueUpdate fieldValue in payload.FieldValues)
    {
      if (string.IsNullOrWhiteSpace(fieldValue.Value))
      {
        fieldValues.Remove(fieldValue.Id);
      }
      else
      {
        fieldValues[fieldValue.Id] = fieldValue.Value;
      }
    }

    invariantOrLocale = new(uniqueName, displayName, description, fieldValues);
    ActorId? actorId = _applicationContext.ActorId;

    if (language == null)
    {
      content.SetInvariant(invariantOrLocale, actorId);
    }
    else
    {
      content.SetLocale(language, invariantOrLocale, actorId);
    }

    await _contentManager.SaveAsync(content, cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
