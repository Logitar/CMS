using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentCommand(Content Content) : IRequest;

internal class SaveContentCommandHandler : IRequestHandler<SaveContentCommand>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;

  public SaveContentCommandHandler(IContentQuerier contentQuerier, IContentRepository contentRepository)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
  }

  public async Task Handle(SaveContentCommand command, CancellationToken cancellationToken)
  {
    Content content = command.Content;

    HashSet<LanguageId?> languageIds = new(capacity: content.Locales.Count + 1);
    foreach (IEvent change in content.Changes)
    {
      if (change is ContentCreated)
      {
        languageIds.Add(null);
      }
      else if (change is ContentLocaleChanged localeChanged)
      {
        languageIds.Add(localeChanged.LanguageId);
      }
    }

    foreach (LanguageId? languageId in languageIds)
    {
      ContentLocale invariantOrLocale = languageId.HasValue ? content.FindLocale(languageId.Value) : content.Invariant;
      ContentId? conflictId = await _contentQuerier.FindIdAsync(content.ContentTypeId, languageId, invariantOrLocale.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
      {
        throw new ContentUniqueNameAlreadyUsedException(content, languageId, invariantOrLocale, conflictId.Value);
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}
