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

  public async Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<StreamId> streamIds = ids.Select(id => id.StreamId);
    return await LoadAsync<FieldType>(streamIds, cancellationToken);
  }

  public async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldTypes, cancellationToken);
  }
}
