using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record CreateOrReplaceContentLocaleCommand(Guid ContentId, Guid? LanguageId, CreateOrReplaceContentLocalePayload Payload)
  : Activity, IRequest<ContentModel?>;

internal class CreateOrReplaceContentLocaleCommandHandler : IRequestHandler<CreateOrReplaceContentLocaleCommand, ContentModel?>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public CreateOrReplaceContentLocaleCommandHandler(
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    ILanguageRepository languageRepository,
    ISender sender)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _languageRepository = languageRepository;
    _sender = sender;
  }

  public async Task<ContentModel?> Handle(CreateOrReplaceContentLocaleCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentLocalePayload payload = command.Payload;
    new CreateOrReplaceContentLocaleValidator().ValidateAndThrow(payload);

    ContentId contentId = new(command.ContentId);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    ContentLocale locale = new(new UniqueName(Content.UniqueNameSettings, payload.UniqueName));
    ActorId actorId = command.GetActorId();

    if (command.LanguageId.HasValue)
    {
      ContentType contentType = await _contentTypeRepository.LoadAsync(content.ContentTypeId, cancellationToken)
        ?? throw new InvalidOperationException($"The content type aggregate 'Id={content.ContentTypeId}' could not be found.");
      if (contentType.IsInvariant)
      {
        throw new CannotCreateInvariantLocaleException(content);
      }

      LanguageId languageId = new(command.LanguageId.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new AggregateNotFoundException(languageId, nameof(command.LanguageId));

      content.SetLocale(language, locale, actorId);
    }
    else
    {
      content.SetInvariant(locale, actorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
