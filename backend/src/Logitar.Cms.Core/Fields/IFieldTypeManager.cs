namespace Logitar.Cms.Core.Fields;

public interface IFieldTypeManager
{
  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
}
