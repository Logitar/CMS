using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldTypeAggregate?> LoadAsync(UniqueNameUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken = default);
}
