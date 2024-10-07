using Logitar.Cms.Contracts.Users;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutUserCommandHandlerTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandHandlerTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should sign-out an user sessions.")]
  public async Task It_should_sign_out_an_user_sessions()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate session1 = new(user);
    SessionAggregate session2 = new(user);
    SessionAggregate signedOut = new(user);
    signedOut.SignOut();
    await _sessionRepository.SaveAsync([session1, session2, signedOut]);

    SignOutUserCommand command = new(user.Id.ToGuid());
    User? result = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Equal(user.Id.ToGuid(), result.Id);

    Assert.Empty(await _sessionRepository.LoadActiveAsync(user));
  }
}
