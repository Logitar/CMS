using Logitar.Cms.Core.Users.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class PasswordChangedHandler : INotificationHandler<PasswordChanged>
{
  private readonly CmsContext _context;

  public PasswordChangedHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(PasswordChanged notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    user.ChangePassword(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
