using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.Localization;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class UpdateContentLocaleCommandHandler : IRequestHandler<UpdateContentLocaleCommand, ContentItem?>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISender _sender;

  public UpdateContentLocaleCommandHandler(IContentQuerier contentQuerier, IContentRepository contentRepository,
    ILanguageRepository languageRepository, ISender sender)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _languageRepository = languageRepository;
    _sender = sender;
  }

  public async Task<ContentItem?> Handle(UpdateContentLocaleCommand command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;

    UpdateContentLocalePayload payload = command.Payload;
    new UpdateContentLocaleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ContentId contentId = new(command.Id);
    ContentAggregate? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    if (command.LanguageId.HasValue)
    {
      LanguageId languageId = new(command.LanguageId.Value);
      LanguageAggregate language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new AggregateNotFoundException<LanguageAggregate>(languageId.AggregateId, nameof(command.LanguageId));

      ContentLocaleUnit locale = content.TryGetLocale(language) ?? throw new ContentLocaleNotFoundException(content, language.Id, nameof(command.LanguageId));
      locale = CreateLocale(payload, uniqueNameSettings, locale);
      content.SetLocale(language, locale, command.ActorId);
    }
    else
    {
      ContentLocaleUnit locale = content.Invariant;
      locale = CreateLocale(payload, uniqueNameSettings, locale);
      content.SetInvariant(locale, command.ActorId);
    }

    await _sender.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }

  private static ContentLocaleUnit CreateLocale(UpdateContentLocalePayload payload, IUniqueNameSettings uniqueNameSettings, ContentLocaleUnit locale)
  {
    UniqueNameUnit uniqueName = payload.UniqueName == null ? locale.UniqueName : new(uniqueNameSettings, payload.UniqueName);
    return new ContentLocaleUnit(uniqueName);
  }
}
