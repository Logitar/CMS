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
      ?? throw new InvalidOperationException($"The content type aggregate 'Id={content.ContentTypeId.Value}' could not be found.");

    Unit unit = await _sender.Send(new ValidateFieldValuesCommand(
      payload.Fields,
      IsInvariant: command.LanguageId.HasValue,
      contentType,
      PropertyName: nameof(payload.Fields)
    ), cancellationToken);
    // TODO(fpion): add field values to content locale
    ContentLocaleUnit locale = new(new UniqueNameUnit(uniqueNameSettings, payload.UniqueName));

    if (command.LanguageId.HasValue)
    {
      if (contentType.IsInvariant)
      {
        throw new CannotCreateInvariantLocaleException(content);
      }

      LanguageId languageId = new(command.LanguageId.Value);
      LanguageAggregate language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new AggregateNotFoundException<LanguageAggregate>(languageId.AggregateId, nameof(command.LanguageId));

      content.SetLocale(language, locale, command.ActorId);
    }
    else
    {
      content.SetInvariant(locale, command.ActorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);
    // TODO(fpion): update field indices (not unique & unique)
    // TODO(fpion): add field values to Contracts ContentLocale

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
