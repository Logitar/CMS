using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record CreateContentCommand(CreateContentPayload Payload) : Activity, IRequest<ContentModel>;

internal class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentModel>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public CreateContentCommandHandler(
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

  public async Task<ContentModel> Handle(CreateContentCommand command, CancellationToken cancellationToken)
  {
    CreateContentPayload payload = command.Payload;

    ContentId? contentId = payload.Id.HasValue ? new(payload.Id.Value) : null;
    if (contentId.HasValue && await _contentRepository.LoadAsync(contentId.Value, cancellationToken) != null)
    {
      throw new ContentIdAlreadyUsedException(contentId.Value, nameof(payload.Id));
    }

    ContentTypeId contentTypeId = new(payload.ContentTypeId);
    ContentType contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException(contentTypeId, nameof(payload.ContentTypeId));

    new CreateContentValidator(contentType).ValidateAndThrow(payload);
    ActorId actorId = command.GetActorId();

    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, payload.UniqueName));
    Content content = new(contentType, invariant, actorId, contentId);

    if (payload.LanguageId.HasValue)
    {
      LanguageId languageId = new(payload.LanguageId.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new AggregateNotFoundException(languageId, nameof(payload.LanguageId));

      content.SetLocale(language, invariant, actorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
