using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal record SaveContentTypeCommand(ContentType ContentType) : IRequest;

internal class SaveContentTypeCommandHandler : IRequestHandler<SaveContentTypeCommand>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public SaveContentTypeCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task Handle(SaveContentTypeCommand command, CancellationToken cancellationToken)
  {
    ContentType contentType = command.ContentType;

    bool hasUniqueNameChanged = false;
    foreach (DomainEvent change in contentType.Changes)
    {
      if (change is ContentType.CreatedEvent || change is ContentType.UpdatedEvent updatedEvent && updatedEvent.UniqueName != null)
      {
        hasUniqueNameChanged = true;
      }
    }

    if (hasUniqueNameChanged)
    {
      ContentTypeId? conflictId = await _contentTypeQuerier.FindIdAsync(contentType.UniqueName, cancellationToken);
      if (conflictId.HasValue && conflictId.Value != contentType.Id)
      {
        throw new UniqueNameAlreadyUsedException(contentType, conflictId.Value);
      }
    }

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);
  }
}
