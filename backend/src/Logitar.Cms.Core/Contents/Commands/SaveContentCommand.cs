using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
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
      UniqueName uniqueName = (languageId.HasValue ? content.FindLocale(languageId.Value) : content.Invariant).UniqueName;
      ContentId? conflictId = await _contentQuerier.FindIdAsync(content.ContentTypeId, languageId, uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}
