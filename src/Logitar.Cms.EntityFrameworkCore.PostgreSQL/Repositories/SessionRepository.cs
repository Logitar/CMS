﻿using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;

internal class SessionRepository : EventStore, ISessionRepository
{
  private static readonly string AggregateType = typeof(SessionAggregate).GetName();

  public SessionRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<IEnumerable<SessionAggregate>> LoadActiveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    string aggregateId = user.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""cms"".""Sessions"" s ON s.""AggregateId"" = e.""AggregateId"" JOIN ""cms"".""Users"" u ON u.""UserId"" = s.""UserId"" WHERE e.""AggregateType"" = {AggregateType} AND s.""IsActive"" = true AND u.""AggregateId"" = {aggregateId}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SessionAggregate>(events);
  }

  public async Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<SessionAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    await base.SaveAsync(session, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sessions, cancellationToken);
  }
}
