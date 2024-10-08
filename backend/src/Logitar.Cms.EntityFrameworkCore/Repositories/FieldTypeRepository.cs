using Logitar.Cms.Core.FieldTypes;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class FieldTypeRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IFieldTypeRepository
{
  public FieldTypeRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<FieldType>(id.AggregateId, version, cancellationToken);
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
