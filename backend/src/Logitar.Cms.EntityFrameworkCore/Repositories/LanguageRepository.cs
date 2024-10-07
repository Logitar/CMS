using Logitar.Cms.Core.Languages;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class LanguageRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ILanguageRepository
{
  private static readonly string AggregateType = typeof(Language).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public LanguageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(id.AggregateId, version, cancellationToken);
  }

  public async Task<Language> LoadDefaultAsync(CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
     .Join(CmsDb.Languages.AggregateId, EventDb.Events.AggregateId,
       new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
     )
     .Where(CmsDb.Languages.IsDefault, Operators.IsEqualTo(true))
     .SelectAll(EventDb.Events.Table)
     .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Language>(events.Select(EventSerializer.Deserialize)).SingleOrDefault()
      ?? throw new InvalidOperationException("The default language aggregate could not be loaded.");
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}
