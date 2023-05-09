using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IMappingService _mappingService;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(CmsContext context, IMappingService mappingService)
  {
    _mappingService = mappingService;
    _sessions = context.Sessions;
  }

  public async Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    SessionEntity entity = await _sessions.AsNoTracking()
      .Include(x => x.User)
      .SingleAsync(x => x.AggregateId == session.Id.Value, cancellationToken);

    return _mappingService.Map<Session>(entity);
  }

  public async Task<Session?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mappingService.Map<Session>(session);
  }

  public async Task<IEnumerable<Session>> GetAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<string> aggregateIds = sessions.Select(session => session.Id.Value).Distinct();

    SessionEntity[] entities = await _sessions.AsNoTracking()
      .Where(x => aggregateIds.Contains(x.AggregateId))
      .ToArrayAsync(cancellationToken);

    return _mappingService.Map<IEnumerable<Session>>(entities);
  }
}
