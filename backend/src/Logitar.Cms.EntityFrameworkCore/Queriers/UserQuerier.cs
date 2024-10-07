using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Users;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _users = context.Users;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'AggregateId={user.Id.AggregateId}' could not be found.");
  }
  public async Task<User?> ReadAsync(UserId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return user == null ? null : await MapAsync(user, cancellationToken);
  }

  private async Task<User> MapAsync(UserEntity user, CancellationToken cancellationToken)
    => (await MapAsync([user], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<User>> MapAsync(IEnumerable<UserEntity> users, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(mapper.ToUser).ToArray();
  }
}
