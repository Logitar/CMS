namespace Logitar.Cms.Core.Users;

public interface IUserRepository
{
  Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
