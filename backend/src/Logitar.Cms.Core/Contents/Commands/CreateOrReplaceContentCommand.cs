using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record CreateOrReplaceContentResult(ContentModel? Content = null, bool Created = false);

public record CreateOrReplaceContentCommand(Guid? ContentId, Guid? LanguageId, CreateOrReplaceContentPayload Payload, long? Version) : IRequest<CreateOrReplaceContentResult>;

internal class CreateOrReplaceContentCommandHandler : IRequestHandler<CreateOrReplaceContentCommand, CreateOrReplaceContentResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMediator _mediator;

  public CreateOrReplaceContentCommandHandler(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    ILanguageRepository languageRepository,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _languageRepository = languageRepository;
    _mediator = mediator;
  }

  public async Task<CreateOrReplaceContentResult> Handle(CreateOrReplaceContentCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentPayload payload = command.Payload;
    new CreateOrReplaceContentValidator().ValidateAndThrow(payload);

    ContentId? contentId = null;
    Content? content = null;
    if (command.ContentId.HasValue)
    {
      contentId = new(command.ContentId.Value);
      content = await _contentRepository.LoadAsync(contentId.Value, cancellationToken);
    }

    bool created = false;
    if (content == null)
    {
      content = await CreateAsync(payload, command.LanguageId, contentId, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(content, payload, command.LanguageId, cancellationToken);
    }

    await _mediator.Send(new SaveContentCommand(content), cancellationToken);

    ContentModel model = await _contentQuerier.ReadAsync(content, cancellationToken);
    return new CreateOrReplaceContentResult(model, created);
  }

  private async Task<Content> CreateAsync(CreateOrReplaceContentPayload payload, Guid? languageGuid, ContentId? contentId, CancellationToken cancellationToken)
  {
    if (!payload.ContentTypeId.HasValue)
    {
      ValidationFailure failure = new(nameof(payload.ContentTypeId), "'ContentTypeId' is required when creating content.", payload.ContentTypeId)
      {
        ErrorCode = "RequiredValidator"
      };
      throw new ValidationException([failure]);
    }

    ContentTypeId contentTypeId = new(payload.ContentTypeId.Value);
    ContentType contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new ContentTypeNotFoundException(contentTypeId, nameof(payload.ContentTypeId));

    string? errorMessage = null;
    if (contentType.IsInvariant && languageGuid.HasValue)
    {
      errorMessage = "'LanguageId' must be null. The content type is invariant.";
    }
    else if (!contentType.IsInvariant && !languageGuid.HasValue)
    {
      errorMessage = "'LanguageId' cannot be null. The content type is not invariant.";
    }
    if (errorMessage != null)
    {
      ValidationFailure failure = new("LanguageId", errorMessage, languageGuid)
      {
        ErrorCode = "InvariantValidator"
      };
      throw new ValidationException([failure]);
    }

    ActorId? actorId = _applicationContext.ActorId;

    ContentLocale invariant = CreateLocale(payload, contentType, language: null);
    Content content = new(contentType, invariant, actorId, contentId);

    if (languageGuid.HasValue)
    {
      LanguageId languageId = new(languageGuid.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, "LanguageId");

      ContentLocale locale = CreateLocale(payload, contentType, language);
      content.SetLocale(language, locale, actorId);
    }

    return content;
  }

  private async Task ReplaceAsync(Content content, CreateOrReplaceContentPayload payload, Guid? languageGuid, CancellationToken cancellationToken)
  {
    Language? language = null;
    if (languageGuid.HasValue)
    {
      ContentType contentType = await _contentTypeRepository.LoadAsync(content, cancellationToken);
      if (contentType.IsInvariant)
      {
        ValidationFailure failure = new("LanguageId", "'LanguageId' must be null. The content type is invariant.", languageGuid)
        {
          ErrorCode = "InvariantValidator"
        };
        throw new ValidationException([failure]);
      }

      LanguageId languageId = new(languageGuid.Value);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, "LanguageId");
    }

    ContentLocale invariantOrLocale = CreateLocale(payload);
    ActorId? actorId = _applicationContext.ActorId;
    if (language == null)
    {
      content.SetInvariant(invariantOrLocale, actorId);
    }
    else
    {
      content.SetLocale(language, invariantOrLocale, actorId);
    }
  }

  private static ContentLocale CreateLocale(CreateOrReplaceContentPayload payload)
  {
    UniqueName uniqueName = new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);

    Dictionary<Guid, string> fieldValues = new(capacity: payload.FieldValues.Count);
    foreach (FieldValue fieldValue in payload.FieldValues)
    {
      if (string.IsNullOrWhiteSpace(fieldValue.Value))
      {
        fieldValues.Remove(fieldValue.Id);
      }
      else
      {
        fieldValues[fieldValue.Id] = fieldValue.Value.Trim();
      }
    }

    return new ContentLocale(uniqueName, displayName, description, fieldValues);
  }
  private static ContentLocale CreateLocale(CreateOrReplaceContentPayload payload, ContentType contentType, Language? language)
  {
    UniqueName uniqueName = new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);

    HashSet<Guid> variantFieldIds = contentType.FieldDefinitions.Where(x => !x.IsInvariant).Select(x => x.Id).ToHashSet();
    Dictionary<Guid, string> fieldValues = new(capacity: payload.FieldValues.Count);
    if (language == null)
    {
      foreach (FieldValue fieldValue in payload.FieldValues)
      {
        if (string.IsNullOrWhiteSpace(fieldValue.Value))
        {
          fieldValues.Remove(fieldValue.Id);
        }
        else if (!variantFieldIds.Contains(fieldValue.Id))
        {
          fieldValues[fieldValue.Id] = fieldValue.Value.Trim();
        }
      }
    }
    else
    {
      foreach (FieldValue fieldValue in payload.FieldValues)
      {
        if (string.IsNullOrWhiteSpace(fieldValue.Value))
        {
          fieldValues.Remove(fieldValue.Id);
        }
        else if (variantFieldIds.Contains(fieldValue.Id))
        {
          fieldValues[fieldValue.Id] = fieldValue.Value.Trim();
        }
      }
    }

    return new ContentLocale(uniqueName, displayName, description, fieldValues);
  }
}
