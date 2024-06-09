using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldTypeAggregate?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldTypeAggregate?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken = default);
  Task<FieldTypeAggregate?> LoadAsync(UniqueNameUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken = default);
}
