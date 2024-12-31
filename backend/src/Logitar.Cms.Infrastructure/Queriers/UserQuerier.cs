using Logitar.Cms.Core.Users;
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
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IdentityContext context)
  {
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
}
