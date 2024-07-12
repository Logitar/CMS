using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
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

  public CreateContentCommandHandler(
    IContentQuerier contentQuerier,
    IContentTypeRepository contentTypeRepository,
    ILanguageRepository languageRepository,
    ISender sender)
  {
    _contentQuerier = contentQuerier;
    _contentTypeRepository = contentTypeRepository;
    _languageRepository = languageRepository;
    _sender = sender;
  }

  public async Task<ContentItem> Handle(CreateContentCommand command, CancellationToken cancellationToken)
  {
    CreateContentPayload payload = command.Payload;

    ContentTypeId contentTypeId = new(payload.ContentTypeId);
    ContentTypeAggregate contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException<ContentTypeAggregate>(contentTypeId.AggregateId, nameof(payload.ContentTypeId));

    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;
    new CreateContentValidator(contentType.IsInvariant, uniqueNameSettings).ValidateAndThrow(payload);

    ContentLocaleUnit invariant = new(new UniqueNameUnit(uniqueNameSettings, payload.UniqueName));
    ContentAggregate content = new(contentType, invariant, command.ActorId);

    if (payload.LanguageId.HasValue)
    {
      LanguageId languageId = new(payload.LanguageId.Value);
      LanguageAggregate language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new AggregateNotFoundException<LanguageAggregate>(languageId.AggregateId, nameof(payload.LanguageId));
      content.SetLocale(language, invariant, command.ActorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
