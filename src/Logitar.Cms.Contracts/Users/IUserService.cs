namespace Logitar.Cms.Contracts.Users;

public interface IUserService
{
  Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken = default);
}
