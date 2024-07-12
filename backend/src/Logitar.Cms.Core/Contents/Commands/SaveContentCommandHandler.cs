using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Languages;
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
      ContentLocaleUnit locale = languageId == null ? content.Invariant : content.GetLocale(languageId);
      ContentAggregate? other = await _contentRepository.LoadAsync(content.ContentTypeId, languageId, locale.UniqueName, cancellationToken);
      if (other != null && !other.Equals(content))
      {
        throw new CmsUniqueNameAlreadyUsedException<ContentAggregate>(languageId, locale.UniqueName, nameof(locale.UniqueName));
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}
