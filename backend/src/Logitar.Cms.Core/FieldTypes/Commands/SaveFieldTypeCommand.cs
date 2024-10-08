using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

internal record SaveFieldTypeCommand(FieldType FieldType) : IRequest;

internal class SaveFieldTypeCommandHandler : IRequestHandler<SaveFieldTypeCommand>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public SaveFieldTypeCommandHandler(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task Handle(SaveFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldType fieldType = command.FieldType;

    bool hasUniqueNameChanged = false;
    foreach (DomainEvent change in fieldType.Changes)
    {
      if (change is FieldType.CreatedEvent || change is FieldType.UpdatedEvent updatedEvent && updatedEvent.UniqueName != null)
      {
        hasUniqueNameChanged = true;
      }
    }

    if (hasUniqueNameChanged)
    {
      FieldTypeId? conflictId = await _fieldTypeQuerier.FindIdAsync(fieldType.UniqueName, cancellationToken);
      if (conflictId.HasValue && conflictId.Value != fieldType.Id)
      {
        throw new UniqueNameAlreadyUsedException(fieldType, conflictId.Value);
      }
    }

    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
