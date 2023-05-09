using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;

internal class UserRepository : EventStore, IUserRepository
{
  private static readonly string AggregateType = typeof(UserAggregate).GetName();

  private readonly ICacheService _cacheService;

  public UserRepository(ICacheService cacheService,
    EventContext context,
    IEventBus eventBus) : base(context, eventBus)
  {
    _cacheService = cacheService;
  }

  public async Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<UserAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(bool includeDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<UserAggregate>(includeDeleted, cancellationToken);
  }

  public async Task<UserAggregate?> LoadAsync(string username, CancellationToken cancellationToken)
  {
    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""cms"".""Users"" u ON u.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND u.""UsernameNormalized"" = {username.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);

    _cacheService.SetActor(user);
  }
}
