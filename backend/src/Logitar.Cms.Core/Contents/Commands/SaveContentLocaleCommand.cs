using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentLocaleCommand(Guid ContentId, Guid? LanguageId, SaveContentLocalePayload Payload, long? Version) : IRequest<ContentModel?>;

internal class SaveContentLocaleCommandHandler : IRequestHandler<SaveContentLocaleCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMediator _mediator;

  public SaveContentLocaleCommandHandler(
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

  public async Task<ContentModel?> Handle(SaveContentLocaleCommand command, CancellationToken cancellationToken)
  {
    SaveContentLocalePayload payload = command.Payload;
    new SaveContentLocaleValidator().ValidateAndThrow(payload);

    ContentId contentId = new(command.ContentId);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    Content reference = (command.Version.HasValue
      ? await _contentRepository.LoadAsync(content.Id, command.Version.Value, cancellationToken)
      : null) ?? content;

    ActorId? actorId = _applicationContext.ActorId;

    UniqueName uniqueName = new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);
    ContentLocale invariantOrLocale = new(uniqueName, displayName, description);

    if (command.LanguageId.HasValue)
    {
      ContentType contentType = await _contentTypeRepository.LoadAsync(content.ContentTypeId, cancellationToken)
        ?? throw new InvalidOperationException($"The content type 'Id={content.ContentTypeId}' was not be loaded.");
      if (contentType.IsInvariant)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }

      LanguageId languageId = new(command.LanguageId.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, nameof(command.LanguageId));

      ContentLocale? existingLocale = content.TryGetLocale(language);
      if (existingLocale == null || !existingLocale.Equals(invariantOrLocale))
      {
        content.SetLocale(language, invariantOrLocale, actorId);
      }
    }
    else if (reference.Invariant != invariantOrLocale)
    {
      content.SetInvariant(invariantOrLocale, actorId);
    }

    await _mediator.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
