using Logitar.Cms.Core.Sessions.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionSignedOutHandler : INotificationHandler<SessionSignedOut>
{
  private readonly CmsContext _context;

  public SessionSignedOutHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionSignedOut notification, CancellationToken cancellationToken)
  {
    SessionEntity user = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{notification.AggregateId}' could not be found.");

    user.SignOut(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
