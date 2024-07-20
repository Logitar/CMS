using Logitar.Cms.Core.FieldTypes;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class FieldTypeRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IFieldTypeRepository
{
  private static readonly string AggregateType = typeof(FieldTypeAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public FieldTypeRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<FieldTypeAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await base.LoadAsync<FieldTypeAggregate>(cancellationToken)).ToArray();
  }

  public async Task<FieldTypeAggregate?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<FieldTypeAggregate>(id.AggregateId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<FieldTypeAggregate>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<AggregateId> aggregateIds = ids.Select(id => id.AggregateId);
    return (await base.LoadAsync<FieldTypeAggregate>(aggregateIds, cancellationToken)).ToArray();
  }

  public async Task<FieldTypeAggregate?> LoadAsync(UniqueNameUnit uniqueName, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.FieldTypes.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(CmsDb.FieldTypes.UniqueNameNormalized, Operators.IsEqualTo(CmsDb.Normalize(uniqueName.Value)))
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<FieldTypeAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<FieldTypeAggregate> fieldTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldTypes, cancellationToken);
  }
}
