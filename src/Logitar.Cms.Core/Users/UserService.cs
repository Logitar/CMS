using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Users.Commands;

namespace Logitar.Cms.Core.Users;

internal class UserService : IUserService
{
  private readonly IRequestPipeline _pipeline;

  public UserService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<User> ChangePasswordAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ChangePassword(id, input), cancellationToken);
  }

  public async Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateUser(id, input), cancellationToken);
  }
}
