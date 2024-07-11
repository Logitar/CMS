using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.FieldTypes;

public interface IFieldTypeRepository
{
  Task<FieldTypeAggregate?> LoadAsync(UniqueNameUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<FieldTypeAggregate> fieldTypes, CancellationToken cancellationToken = default);
}
