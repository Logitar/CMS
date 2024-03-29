﻿using Logitar.Cms.Core.Localization;
using Logitar.Data;
using Logitar.EventSourcing;
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

  public LanguageRepository(ISqlHelper sqlHelper, IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<LanguageAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<LanguageAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<LanguageAggregate?> LoadAsync(LocaleUnit locale, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.Languages.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(CmsDb.Languages.LocaleNormalized, Operators.IsEqualTo(locale.Code.ToUpper()))
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<LanguageAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
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
