using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;

internal class UserRepository : EventStore, IUserRepository
{
  public UserRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);
  }
}
