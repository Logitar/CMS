namespace Logitar.Cms.Contracts.Users;

public interface IUserService
{
  Task<User> ChangePasswordAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken = default);
  Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken = default);
}
