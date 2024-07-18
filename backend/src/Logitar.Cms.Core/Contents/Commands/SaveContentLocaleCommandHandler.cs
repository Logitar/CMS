using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class SaveContentLocaleCommandHandler : IRequestHandler<SaveContentLocaleCommand, ContentItem?>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public SaveContentLocaleCommandHandler(
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

  public async Task<ContentItem?> Handle(SaveContentLocaleCommand command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;

    SaveContentLocalePayload payload = command.Payload;
    new SaveContentLocaleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ContentId contentId = new(command.ContentId);
    ContentAggregate? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    ContentTypeAggregate contentType = await _contentTypeRepository.LoadAsync(content.ContentTypeId, cancellationToken)
      ?? throw new InvalidOperationException($"The content type aggregate 'Id={content.ContentTypeId}' could not be found.");

    LanguageAggregate? language = null;
    if (command.LanguageId.HasValue)
    {
      if (contentType.IsInvariant)
      {
        throw new CannotCreateInvariantLocaleException(content);
      }

      LanguageId languageId = new(command.LanguageId.Value);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new AggregateNotFoundException<LanguageAggregate>(languageId.AggregateId, nameof(command.LanguageId));
    }

    ValidateFieldValuesCommand validateFieldValues = new(payload.Fields, contentType, content, language, PropertyName: nameof(payload.Fields));
    await _sender.Send(validateFieldValues, cancellationToken);
    Dictionary<Guid, string> fieldValues = new(capacity: payload.Fields.Count);
    foreach (FieldValuePayload field in payload.Fields)
    {
      fieldValues[field.Id] = field.Value;
    }
    ContentLocaleUnit locale = new(new UniqueNameUnit(uniqueNameSettings, payload.UniqueName), fieldValues);

    if (language == null)
    {
      content.SetInvariant(locale, command.ActorId);
    }
    else
    {
      content.SetLocale(language, locale, command.ActorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);
    // TODO(fpion): add field values to Contracts ContentLocale

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}

// TODO(fpion): unit tests
// TODO(fpion): integration tests
