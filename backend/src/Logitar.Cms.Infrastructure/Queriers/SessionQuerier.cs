using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _sessions = context.Sessions;
  }

  public async Task<SessionModel> ReadAsync(Session session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity 'StreamId={session.Id.Value}' could not be found.");
  }
  public async Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new SessionId(tenantId: null, new EntityId(id)), cancellationToken);
  }
  public async Task<SessionModel?> ReadAsync(SessionId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return session == null ? null : await MapAsync(session, cancellationToken);
  }

  private async Task<SessionModel> MapAsync(SessionEntity session, CancellationToken cancellationToken)
  {
    return (await MapAsync([session], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SessionModel>> MapAsync(IEnumerable<SessionEntity> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(mapper.ToSession).ToArray();
  }
}
