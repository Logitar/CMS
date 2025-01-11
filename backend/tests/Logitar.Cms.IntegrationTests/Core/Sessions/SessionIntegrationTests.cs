using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Sessions.Queries;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions;

[Trait(Traits.Category, Categories.Integration)]
public class SessionIntegrationTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SessionIntegrationTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should read the session found by ID.")]
  public async Task Given_SessionFound_When_Read_Then_SessionReturned()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    EntityId entityId = EntityId.NewId();
    Session session = user.SignIn(secret, actorId: null, entityId);
    session.SetCustomAttribute(new Identifier("IpAddress"), Faker.Internet.Ip());
    session.SetCustomAttribute(new Identifier("AdditionalInformation"), $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}");
    session.Update(ActorId);
    await _sessionRepository.SaveAsync(session);

    ReadSessionQuery query = new(entityId.ToGuid());
    SessionModel? model = await Mediator.Send(query);
    Assert.NotNull(model);
    Assert.Equal(entityId.ToGuid(), model.Id);
    Assert.Equal(user.EntityId.ToGuid(), model.User.Id);
    Assert.True(model.IsPersistent);
    Assert.True(model.IsActive);
  }

  [Fact(DisplayName = "It should renew a session given valid credentials.")]
  public async Task Given_ValidCredentials_When_Renew_Then_SessionRenewed()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    EntityId entityId = EntityId.NewId();
    Session session = user.SignIn(secret, actorId: null, entityId);
    session.SetCustomAttribute(new Identifier("IpAddress"), "0.0.0.0/0");
    session.Update(ActorId);
    await _sessionRepository.SaveAsync(session);

    RefreshToken refreshToken = new(entityId.ToGuid(), secretString);
    RenewSessionPayload payload = new(refreshToken.Encode(),
    [
      new CustomAttribute("IpAddress", Faker.Internet.Ip()),
      new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}")
    ]);
    RenewSessionCommand command = new(payload);
    SessionModel model = await Mediator.Send(command);

    Assert.Equal(refreshToken.SessionId, model.Id);
    Assert.Equal(session.Version + 2, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(DateTime.UtcNow, model.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(user.EntityId.ToGuid(), model.User.Id);
    Assert.True(model.IsPersistent);
    Assert.NotNull(model.RefreshToken);
    Assert.NotEqual(payload.RefreshToken, model.RefreshToken);
    refreshToken = RefreshToken.Decode(model.RefreshToken);
    Assert.Equal(model.Id, refreshToken.SessionId);
    Assert.Equal(RefreshToken.SecretLength, Convert.FromBase64String(refreshToken.Secret).Length);
    Assert.True(model.IsActive);
    Assert.Null(model.SignedOutBy);
    Assert.Null(model.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, model.CustomAttributes);
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

  [Fact(DisplayName = "It should sign-out an active user session.")]
  public async Task Given_ActiveSession_When_SignOut_Then_SessionIsSignedOut()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    Session session = new(user);
    await _sessionRepository.SaveAsync(session);

    SignOutSessionCommand command = new(session.EntityId.ToGuid());
    SessionModel? model = await Mediator.Send(command);

    Assert.NotNull(model);
    Assert.Equal(command.Id, model.Id);
    Assert.False(model.IsActive);
    Assert.Equal(Actor, model.SignedOutBy);
    Assert.True(model.SignedOutOn.HasValue);
    Assert.Equal(DateTime.UtcNow, model.SignedOutOn.Value, TimeSpan.FromMinutes(1));
  }
}
