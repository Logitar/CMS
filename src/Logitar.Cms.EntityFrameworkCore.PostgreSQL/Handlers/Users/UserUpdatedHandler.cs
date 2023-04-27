using Logitar.Cms.Core.Users.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class UserUpdatedHandler : INotificationHandler<UserUpdated>
{
  private readonly CmsContext _context;

  public UserUpdatedHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(UserUpdated notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    user.Update(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
