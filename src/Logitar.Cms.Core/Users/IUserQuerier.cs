using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Users;

public interface IUserQuerier
{
  Task<User> GetAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
