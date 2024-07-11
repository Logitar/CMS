using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class AuthenticateUserCommandHandlerTests : IntegrationTests
{
  public AuthenticateUserCommandHandlerTests() : base()
  {
  }

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task It_should_authenticate_the_user()
  {
    AuthenticateUserPayload payload = new(UsernameString, PasswordString);
    AuthenticateUserCommand command = new(payload);
    User user = await Pipeline.ExecuteAsync(command);

    Assert.Equal(UsernameString, user.UniqueName);
    Assert.NotNull(user.AuthenticatedOn);
  }
}
