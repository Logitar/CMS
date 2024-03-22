using Logitar.Cms.Core.Archetypes;
using Logitar.Cms.Core.Shared;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ArchetypeRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IArchetypeRepository
{
  private static readonly string AggregateType = typeof(ArchetypeAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ArchetypeRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ArchetypeAggregate?> LoadAsync(IdentifierUnit uniqueName, CancellationToken cancellationToken)
  {
    IQueryBuilder query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.Archetypes.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(CmsDb.Archetypes.UniqueNameNormalized, Operators.IsEqualTo(uniqueName.Value.ToUpper()));

    EventEntity[] events = await EventContext.Events.AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ArchetypeAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ArchetypeAggregate archetype, CancellationToken cancellationToken)
  {
    await base.SaveAsync(archetype, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ArchetypeAggregate> archetypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(archetypes, cancellationToken);
  }
}
