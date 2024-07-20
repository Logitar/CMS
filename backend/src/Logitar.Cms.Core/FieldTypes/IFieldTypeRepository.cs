using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.FieldTypes;

public interface IFieldTypeRepository
{
  Task<IReadOnlyCollection<FieldTypeAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<FieldTypeAggregate?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FieldTypeAggregate>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken = default);
  Task<FieldTypeAggregate?> LoadAsync(UniqueNameUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<FieldTypeAggregate> fieldTypes, CancellationToken cancellationToken = default);
}
