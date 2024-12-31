using Logitar.Cms.Core.Users.Models;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;

namespace Logitar.Cms.Core.Users;

public interface IUserQuerier
{
  Task<bool> AnyAsync(CancellationToken cancellationToken = default);

  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<UserModel> ReadAsync(User user, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
