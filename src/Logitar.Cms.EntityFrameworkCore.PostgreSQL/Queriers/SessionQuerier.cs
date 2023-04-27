using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly CmsContext _context;
  private readonly IMappingService _mappingService;

  public SessionQuerier(CmsContext context, IMappingService mappingService)
  {
    _context = context;
    _mappingService = mappingService;
  }

  public async Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    SessionEntity entity = await _context.Sessions.AsNoTracking()
      .Include(x => x.User)
      .SingleAsync(x => x.AggregateId == session.Id.Value, cancellationToken);

    return _mappingService.Map<Session>(entity);
  }

  public async Task<Session?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SessionEntity? session = await _context.Sessions.AsNoTracking()
      .Include(x => x.User)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mappingService.Map<Session>(session);
  }
}
