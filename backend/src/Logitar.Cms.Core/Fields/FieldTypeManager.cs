using Logitar.Cms.Core.Fields.Events;

namespace Logitar.Cms.Core.Fields;

internal class FieldTypeManager : IFieldTypeManager
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public FieldTypeManager(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = fieldType.Changes.Any(change => change is FieldTypeUniqueNameChanged);
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
