using Logitar.Cms.Contracts.Users;
using Logitar.Identity.Domain.Users;

namespace Logitar.Cms.Core.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
