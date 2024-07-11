using Logitar.Cms.Core;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ContentTypeRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IContentTypeRepository
{
  private static readonly string AggregateType = typeof(ContentTypeAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ContentTypeRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ContentTypeAggregate?> LoadAsync(IdentifierUnit uniqueName, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.ContentTypes.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(CmsDb.ContentTypes.UniqueNameNormalized, Operators.IsEqualTo(CmsDb.Normalize(uniqueName.Value)))
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ContentTypeAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ContentTypeAggregate> contentTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentTypes, cancellationToken);
  }
}
