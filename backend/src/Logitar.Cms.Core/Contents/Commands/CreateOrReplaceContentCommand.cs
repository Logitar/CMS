using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
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
      ValidationFailure failure = new(nameof(payload.ContentTypeId), "'{PropertyName}' is required when creating content.", payload.ContentTypeId)
      {
        ErrorCode = "RequiredValidator"
      };
      throw new ValidationException([failure]);
    }

    ContentTypeId contentTypeId = new(payload.ContentTypeId.Value);
    ContentType contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new ContentTypeNotFoundException(contentTypeId, nameof(payload.ContentTypeId));

    if (contentType.IsInvariant && languageGuid.HasValue)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }
    else if (!contentType.IsInvariant && !languageGuid.HasValue)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    ContentLocale invariantAndLocale = CreateLocale(payload);
    ActorId? actorId = _applicationContext.ActorId;

    Content content = new(contentType, invariantAndLocale, actorId, contentId);

    if (languageGuid.HasValue)
    {
      LanguageId languageId = new(languageGuid.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, "LanguageId");

      content.SetLocale(language, invariantAndLocale, actorId);
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
        throw new NotImplementedException(); // TODO(fpion): typed exception
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

    return new ContentLocale(uniqueName, displayName, description);
  }
}
