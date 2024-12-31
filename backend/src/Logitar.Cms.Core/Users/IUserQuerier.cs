using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;

namespace Logitar.Cms.Core.Users;

public interface IUserQuerier
{
  Task<bool> AnyAsync(CancellationToken cancellationToken = default);

  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
}
