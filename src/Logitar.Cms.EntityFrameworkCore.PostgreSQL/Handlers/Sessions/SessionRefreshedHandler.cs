using Logitar.Cms.Core.Sessions.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionRefreshedHandler : INotificationHandler<SessionRefreshed>
{
  private readonly CmsContext _context;

  public SessionRefreshedHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionRefreshed notification, CancellationToken cancellationToken)
  {
    SessionEntity user = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{notification.AggregateId}' could not be found.");

    user.Refresh(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
