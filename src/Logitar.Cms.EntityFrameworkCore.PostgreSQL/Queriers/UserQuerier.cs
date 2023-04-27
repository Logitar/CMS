using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.Core.Users;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IMappingService _mappingService;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(CmsContext context, IMappingService mappingService)
  {
    _mappingService = mappingService;
    _users = context.Users;
  }

  public async Task<User> GetAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    UserEntity entity = await _users.AsNoTracking()
     .SingleAsync(x => x.AggregateId == user.Id.Value, cancellationToken);

    return _mappingService.Map<User>(entity);
  }
}
