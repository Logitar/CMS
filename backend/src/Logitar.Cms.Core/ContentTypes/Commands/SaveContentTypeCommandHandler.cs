using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Cms.Core.Shared;
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

    if (contentType.Changes.Any(change => change is ContentTypeCreatedEvent))
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
