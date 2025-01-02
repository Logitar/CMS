using Logitar.Cms.Core.Contents.Events;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentTypeCommand(ContentType ContentType) : IRequest;

internal class SaveContentTypeCommandHandler : IRequestHandler<SaveContentTypeCommand>
{
  private readonly IContentTypeQuerier _fieldTypeQuerier;
  private readonly IContentTypeRepository _fieldTypeRepository;

  public SaveContentTypeCommandHandler(IContentTypeQuerier fieldTypeQuerier, IContentTypeRepository fieldTypeRepository)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task Handle(SaveContentTypeCommand command, CancellationToken cancellationToken)
  {
    ContentType contentType = command.ContentType;

    bool hasUniqueNameChanged = contentType.Changes.Any(change => change is ContentTypeCreated || change is ContentTypeUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      ContentTypeId? conflictId = await _fieldTypeQuerier.FindIdAsync(contentType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(contentType.Id))
      {
        throw new UniqueNameAlreadyUsedException(contentType, conflictId.Value);
      }
    }

    await _fieldTypeRepository.SaveAsync(contentType, cancellationToken);
  }
}
