using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record CreateContentCommand(CreateContentPayload Payload) : IRequest<ContentModel>;

internal class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMediator _mediator;

  public CreateContentCommandHandler(
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

  public async Task<ContentModel> Handle(CreateContentCommand command, CancellationToken cancellationToken)
  {
    CreateContentPayload payload = command.Payload;
    new CreateContentValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(payload.ContentTypeId);
    ContentType contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new ContentTypeNotFoundException(contentTypeId, nameof(payload.ContentTypeId));

    ActorId? actorId = _applicationContext.ActorId;

    UniqueName uniqueName = new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);
    ContentLocale invariantAndLocale = new(uniqueName, displayName, description);

    ContentId? contentId = null;
    Content? content;
    if (payload.Id.HasValue)
    {
      contentId = new(payload.Id.Value);
      content = await _contentRepository.LoadAsync(contentId.Value, cancellationToken);
      if (content != null)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
    }
    content = new(contentType, invariantAndLocale, actorId, contentId);

    if (contentType.IsInvariant)
    {
      if (payload.LanguageId.HasValue)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
    }
    else if (!payload.LanguageId.HasValue)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }
    else
    {
      LanguageId languageId = new(payload.LanguageId.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, nameof(payload.LanguageId));

      content.SetLocale(language, invariantAndLocale, actorId);
    }

    await _mediator.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
