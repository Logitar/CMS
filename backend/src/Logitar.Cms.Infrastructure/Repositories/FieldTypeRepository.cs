using Logitar.Cms.Core.Fields;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Repositories;

internal class FieldTypeRepository : Repository, IFieldTypeRepository
{
  public FieldTypeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<FieldType>(id.StreamId, version, cancellationToken);
  }
}
