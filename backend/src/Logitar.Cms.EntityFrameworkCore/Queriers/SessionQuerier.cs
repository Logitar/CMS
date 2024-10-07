using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _sessions = context.Sessions;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity 'AggregateId={session.Id.AggregateId}' could not be found.");
  }
  public async Task<Session?> ReadAsync(SessionId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return session == null ? null : await MapAsync(session, cancellationToken);
  }

  private async Task<Session> MapAsync(SessionEntity session, CancellationToken cancellationToken)
    => (await MapAsync([session], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<Session>> MapAsync(IEnumerable<SessionEntity> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(mapper.ToSession).ToArray();
  }
}
