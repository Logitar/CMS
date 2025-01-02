using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record UpdateContentLocaleCommand(Guid ContentId, Guid? LanguageId, UpdateContentLocalePayload Payload) : IRequest<ContentModel?>;

internal class UpdateContentLocaleCommandHandler : IRequestHandler<UpdateContentLocaleCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMediator _mediator;

  public UpdateContentLocaleCommandHandler(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    ILanguageRepository languageRepository,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _languageRepository = languageRepository;
    _mediator = mediator;
  }

  public async Task<ContentModel?> Handle(UpdateContentLocaleCommand command, CancellationToken cancellationToken)
  {
    UpdateContentLocalePayload payload = command.Payload;
    new UpdateContentLocaleValidator().ValidateAndThrow(payload);

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

    ActorId? actorId = _applicationContext.ActorId;

    UniqueName uniqueName = string.IsNullOrWhiteSpace(payload.UniqueName) ? invariantOrLocale.UniqueName : new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = payload.DisplayName == null ? invariantOrLocale.DisplayName : DisplayName.TryCreate(payload.DisplayName.Value);
    Description? description = payload.Description == null ? invariantOrLocale.Description : Description.TryCreate(payload.Description.Value);
    invariantOrLocale = new(uniqueName, displayName, description);

    if (language == null)
    {
      content.SetInvariant(invariantOrLocale, actorId);
    }
    else
    {
      content.SetLocale(language, invariantOrLocale, actorId);
    }

    await _mediator.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
