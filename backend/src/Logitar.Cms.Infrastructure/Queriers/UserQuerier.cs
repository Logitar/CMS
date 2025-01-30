using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Users;
using Logitar.Cms.Core.Users.Models;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _users = context.Users;
  }

  public async Task<bool> AnyAsync(CancellationToken cancellationToken)
  {
    return await _users.AsNoTracking().AnyAsync(cancellationToken);
  }

  public async Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName.Value);

    string? streamId = await _users.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new UserId(new StreamId(streamId));
  }

  public async Task<UserModel> ReadAsync(User user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'StreamId={user.Id.Value}' could not be found.");
  }
  public async Task<UserModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new UserId(tenantId: null, new EntityId(id)), cancellationToken);
  }
  public async Task<UserModel?> ReadAsync(UserId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return user == null ? null : await MapAsync(user, cancellationToken);
  }
  public async Task<UserModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return user == null ? null : await MapAsync(user, cancellationToken);
  }

  private async Task<UserModel> MapAsync(UserEntity user, CancellationToken cancellationToken)
  {
    return (await MapAsync([user], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<UserModel>> MapAsync(IEnumerable<UserEntity> users, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(mapper.ToUser).ToArray();
  }
}
