using Logitar.Cms.Core.Users.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class UserCreatedHandler : INotificationHandler<UserCreated>
{
  private readonly CmsContext _context;

  public UserCreatedHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(UserCreated notification, CancellationToken cancellationToken)
  {
    UserEntity user = new(notification);

    _context.Users.Add(user);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
