using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Core.Users;

[Trait(Traits.Category, Categories.Integration)]
public class UserIntegrationTests : IntegrationTests
{
  public UserIntegrationTests() : base()
  {
  }

  [Fact(DisplayName = "It should authenticate an user given valid credentials.")]
  public async Task Given_ValidCredentials_When_Authenticate_Then_UserIsAuthenticated()
  {
    AuthenticateUserPayload payload = new(UniqueName, Password);
    AuthenticateUserCommand command = new(payload);
    UserModel user = await Mediator.Send(command);

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value, TimeSpan.FromMinutes(1));
  }
}
