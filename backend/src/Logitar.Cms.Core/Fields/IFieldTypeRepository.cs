namespace Logitar.Cms.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken = default);
}
