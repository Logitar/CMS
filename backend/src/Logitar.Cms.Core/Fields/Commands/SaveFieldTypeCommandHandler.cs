using Logitar.Cms.Core.Fields.Events;
using Logitar.Cms.Core.Shared;
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

    if (fieldType.Changes.Any(change => change is FieldTypeCreatedEvent))
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
