﻿using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ContentRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IContentRepository
{
  private static readonly string AggregateType = typeof(ContentAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ContentRepository(ISqlHelper sqlHelper, IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ContentAggregate?> LoadAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueNameUnit uniqueName, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table).SelectAll(EventDb.Events.Table)
      .Join(CmsDb.ContentItems.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(CmsDb.ContentTypes.ContentTypeId, CmsDb.ContentItems.ContentTypeId)
      .Join(CmsDb.ContentLocales.ContentItemId, CmsDb.ContentItems.ContentItemId)
      .LeftJoin(CmsDb.Languages.LanguageId, CmsDb.ContentLocales.LanguageId)
      .Where(CmsDb.ContentTypes.AggregateId, Operators.IsEqualTo(contentTypeId.Value))
      .Where(CmsDb.Languages.AggregateId, languageId == null ? Operators.IsNull() : Operators.IsEqualTo(languageId.Value))
      .Where(CmsDb.ContentLocales.UniqueNameNormalized, Operators.IsEqualTo(uniqueName.Value.ToUpper()))
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ContentAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ContentAggregate content, CancellationToken cancellationToken)
  {
    await base.SaveAsync(content, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ContentAggregate> contents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contents, cancellationToken);
  }
}
