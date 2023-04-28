using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;

internal class UserRepository : EventStore, IUserRepository
{
  private readonly ICacheService _cacheService;

  public UserRepository(ICacheService cacheService,
    EventContext context,
    IEventBus eventBus) : base(context, eventBus)
  {
    _cacheService = cacheService;
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(bool includeDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<UserAggregate>(includeDeleted, cancellationToken);
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);

    _cacheService.SetActor(user);
  }
}
