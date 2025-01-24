using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record PublishContentCommand(Guid ContentId, Guid? LanguageId) : IRequest<ContentModel?>;

internal class PublishContentCommandHandler : IRequestHandler<PublishContentCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentManager _contentManager;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;

  public PublishContentCommandHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository)
  {
    _applicationContext = applicationContext;
    _contentManager = contentManager;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
  }

  public async Task<ContentModel?> Handle(PublishContentCommand command, CancellationToken cancellationToken)
  {
    ContentId contentId = new(command.ContentId);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;
    if (command.LanguageId.HasValue)
    {
      LanguageId languageId = new(command.LanguageId.Value);
      if (!content.PublishLocale(languageId))
      {
        return null;
      }
    }
    else
    {
      content.PublishInvariant(actorId);
    }

    await _contentManager.SaveAsync(content, cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
