using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class SaveContentCommandHandler : IRequestHandler<SaveContentCommand>
{
  private readonly IContentRepository _contentRepository;

  public SaveContentCommandHandler(IContentRepository contentRepository)
  {
    _contentRepository = contentRepository;
  }

  public async Task Handle(SaveContentCommand command, CancellationToken cancellationToken)
  {
    ContentAggregate content = command.Content;

    HashSet<LanguageId?> changedLocales = [];
    foreach (DomainEvent change in content.Changes)
    {
      if (change is ContentCreatedEvent)
      {
        changedLocales.Add(null);
      }
      else if (change is ContentLocaleChangedEvent localeChanged)
      {
        changedLocales.Add(localeChanged.LanguageId);
      }
    }

    foreach (LanguageId? languageId in changedLocales)
    {
      ContentLocaleUnit locale = languageId == null ? content.Invariant : content.GetLocale(languageId)
        ?? throw new InvalidOperationException($"The content locale 'ContentId={content.Id.Value}, LanguageId={languageId?.Value ?? "<null>"}' could not be found.");
      ContentAggregate? other = await _contentRepository.LoadAsync(content.ContentTypeId, languageId, locale.UniqueName, cancellationToken);
      if (other != null && !other.Equals(content))
      {
        throw new UniqueNameAlreadyUsedException<ContentAggregate>(languageId, locale.UniqueName, nameof(locale.UniqueName));
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}
