using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentItem>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public CreateContentCommandHandler(IContentQuerier contentQuerier, IContentTypeRepository contentTypeRepository, ILanguageRepository languageRepository, ISender sender)
  {
    _contentQuerier = contentQuerier;
    _contentTypeRepository = contentTypeRepository;
    _languageRepository = languageRepository;
    _sender = sender;
  }

  public async Task<ContentItem> Handle(CreateContentCommand command, CancellationToken cancellationToken)
  {
    CreateContentPayload payload = command.Payload;
    ContentTypeAggregate contentType = await _contentTypeRepository.LoadAsync(payload.ContentTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException<ContentTypeAggregate>(payload.ContentTypeId, nameof(payload.ContentTypeId));

    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;
    new CreateContentValidator(contentType.IsInvariant, uniqueNameSettings).ValidateAndThrow(payload);

    UniqueNameUnit uniqueName = new(uniqueNameSettings, payload.UniqueName);
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);

    ContentLocaleUnit locale = new(uniqueName, displayName, description);
    ContentAggregate content = new(contentType, locale, command.ActorId);

    if (payload.LanguageId.HasValue)
    {
      LanguageAggregate language = await _languageRepository.LoadAsync(payload.LanguageId.Value, cancellationToken)
        ?? throw new AggregateNotFoundException<LanguageAggregate>(payload.LanguageId.Value, nameof(payload.LanguageId));
      content.SetLocale(language, locale, command.ActorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
