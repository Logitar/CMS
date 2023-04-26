using Logitar.Cms.Core.Sessions.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionCreatedHandler : INotificationHandler<SessionCreated>
{
  private readonly CmsContext _context;

  public SessionCreatedHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionCreated notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.ActorId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{notification.ActorId}' could not be found.");

    SessionEntity session = new(notification, user);

    _context.Sessions.Add(session);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
