using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class SaveContentTypeCommandHandler : IRequestHandler<SaveContentTypeCommand>
{
  private readonly IContentTypeRepository _contentTypeRepository;

  public SaveContentTypeCommandHandler(IContentTypeRepository contentTypeRepository)
  {
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task Handle(SaveContentTypeCommand command, CancellationToken cancellationToken)
  {
    ContentTypeAggregate contentType = command.ContentType;

    bool uniqueNameHasChanged = false;

    foreach (DomainEvent change in contentType.Changes)
    {
      if (change is ContentTypeCreatedEvent || (change is ContentTypeUpdatedEvent updated && updated.UniqueName != null))
      {
        uniqueNameHasChanged = true;
      }
    }

    if (uniqueNameHasChanged)
    {
      ContentTypeAggregate? other = await _contentTypeRepository.LoadAsync(contentType.UniqueName, cancellationToken);
      if (other != null && !other.Equals(contentType))
      {
        throw new UniqueNameAlreadyUsedException<ContentTypeAggregate>(contentType.UniqueName, nameof(contentType.UniqueName));
      }
    }

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);
  }
}
