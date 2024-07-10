using Logitar.Cms.Core.Languages;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class LanguageRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ILanguageRepository
{
  private static readonly string AggregateType = typeof(LanguageAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public LanguageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<LanguageAggregate?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<LanguageAggregate>(id.AggregateId, cancellationToken);
  }

  public async Task<LanguageAggregate?> LoadAsync(LocaleUnit locale, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.Languages.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(CmsDb.Languages.CodeNormalized, Operators.IsEqualTo(CmsDb.Normalize(locale.Code)))
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<LanguageAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<LanguageAggregate> LoadDefaultAsync(CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.Languages.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(CmsDb.Languages.IsDefault, Operators.IsEqualTo(true))
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<LanguageAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault()
      ?? throw new InvalidOperationException("The default language aggregate could not be found.");
  }

  public async Task SaveAsync(LanguageAggregate language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<LanguageAggregate> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}
