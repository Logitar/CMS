using Logitar.Cms.Core.Fields.Events;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record SaveFieldTypeCommand(FieldType FieldType) : IRequest;

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

    bool hasUniqueNameChanged = fieldType.Changes.Any(change => change is FieldTypeCreated || change is FieldTypeUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      FieldTypeId? conflictId = await _fieldTypeQuerier.FindIdAsync(fieldType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(fieldType.Id))
      {
        throw new UniqueNameAlreadyUsedException(fieldType, conflictId.Value);
      }
    }

    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
