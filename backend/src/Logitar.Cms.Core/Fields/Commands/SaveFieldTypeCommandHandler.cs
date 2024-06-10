using Logitar.Cms.Core.Fields.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

internal class SaveFieldTypeCommandHandler : IRequestHandler<SaveFieldTypeCommand>
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public SaveFieldTypeCommandHandler(IFieldTypeRepository fieldTypeRepository)
  {
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task Handle(SaveFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldTypeAggregate fieldType = command.FieldType;

    bool uniqueNameHasChanged = false;

    foreach (DomainEvent change in fieldType.Changes)
    {
      if (change is FieldTypeCreatedEvent || (change is FieldTypeUpdatedEvent updated && updated.UniqueName != null))
      {
        uniqueNameHasChanged = true;
      }
    }

    if (uniqueNameHasChanged)
    {
      FieldTypeAggregate? other = await _fieldTypeRepository.LoadAsync(fieldType.UniqueName, cancellationToken);
      if (other != null && !other.Equals(fieldType))
      {
        throw new UniqueNameAlreadyUsedException<FieldTypeAggregate>(fieldType.UniqueName, nameof(fieldType.UniqueName));
      }
    }

    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
