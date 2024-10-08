using Logitar.Cms.Contracts.Users;
using Logitar.Identity.Domain.Users;

namespace Logitar.Cms.Core.Users;

public interface IUserQuerier
{
  Task<UserModel> ReadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
