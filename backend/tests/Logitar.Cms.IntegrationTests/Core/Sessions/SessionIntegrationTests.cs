using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Core.Sessions;

[Trait(Traits.Category, Categories.Integration)]
public class SessionIntegrationTests : IntegrationTests
{
  public SessionIntegrationTests() : base()
  {
  }

  [Fact(DisplayName = "It should sign-in an user given valid credentials.")]
  public async Task Given_ValidCredentials_When_SignIn_Then_UserIsSignedIn()
  {
    SignInSessionPayload payload = new(UniqueName, Password, Guid.NewGuid(), isPersistent: true,
    [
      new CustomAttribute("IpAddress", Faker.Internet.Ip()),
      new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}")
    ]);
    SignInSessionCommand command = new(payload);
    SessionModel session = await Mediator.Send(command);

    Assert.Equal(payload.Id, session.Id);
    Assert.Equal(2, session.Version);
    Assert.Equal(Actor, session.CreatedBy);
    Assert.Equal(DateTime.UtcNow, session.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, session.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, session.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.IsPersistent, session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    RefreshToken refreshToken = RefreshToken.Decode(session.RefreshToken);
    Assert.Equal(session.Id, refreshToken.SessionId);
    Assert.Equal(RefreshToken.SecretLength, Convert.FromBase64String(refreshToken.Secret).Length);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(payload.UniqueName, session.User.UniqueName);

    UserEntity? user = await IdentityContext.Users.SingleOrDefaultAsync();
    Assert.NotNull(user);
    Assert.Equal(ActorId.Value, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn, TimeSpan.FromMinutes(1));
    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value, TimeSpan.FromMinutes(1));
  }
}
