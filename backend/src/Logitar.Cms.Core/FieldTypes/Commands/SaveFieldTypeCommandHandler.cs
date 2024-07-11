using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

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

    bool hasUniqueNameChanged = false;
    foreach (DomainEvent change in fieldType.Changes)
    {
      if (change is FieldTypeCreatedEvent)
      {
        hasUniqueNameChanged = true;
      }
    }

    if (hasUniqueNameChanged)
    {
      FieldTypeAggregate? other = await _fieldTypeRepository.LoadAsync(fieldType.UniqueName, cancellationToken);
      if (other != null && !other.Equals(fieldType))
      {
        throw new UniqueNameAlreadyUsedException<FieldTypeAggregate>(tenantId: null, fieldType.UniqueName); // TODO(fpion): refactor
      }
    }

    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
