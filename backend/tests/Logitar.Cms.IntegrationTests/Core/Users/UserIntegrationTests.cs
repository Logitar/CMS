using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Core.Users.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Users;

[Trait(Traits.Category, Categories.Integration)]
public class UserIntegrationTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public UserIntegrationTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
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

  [Fact(DisplayName = "It should sign-out the active user sessions.")]
  public async Task Given_ActiveSessions_When_SignOut_Then_SessionsSignedOut()
  {
    ActorId actorId = new("SYSTEM");

    User user = Assert.Single(await _userRepository.LoadAsync());

    Session session1 = new(user);
    Session session2 = new(user);
    Session signedOut = new(user);
    signedOut.SignOut(actorId);
    await _sessionRepository.SaveAsync([session1, session2, signedOut]);

    SignOutUserCommand command = new(user.EntityId.ToGuid());
    UserModel? model = await Mediator.Send(command);
    Assert.NotNull(model);
    Assert.Equal(command.Id, model.Id);

    SessionEntity[] sessions = await IdentityContext.Sessions.AsNoTracking().ToArrayAsync();
    Assert.Equal(3, sessions.Length);
    Assert.Contains(sessions, s => s.StreamId == session1.Id.Value && !s.IsActive && s.SignedOutBy == user.Id.Value && (DateTime.UtcNow - s.CreatedOn < TimeSpan.FromMinutes(1)));
    Assert.Contains(sessions, s => s.StreamId == session2.Id.Value && !s.IsActive && s.SignedOutBy == user.Id.Value && (DateTime.UtcNow - s.CreatedOn < TimeSpan.FromMinutes(1)));
    Assert.Contains(sessions, s => s.StreamId == signedOut.Id.Value && !s.IsActive && s.SignedOutBy == actorId.Value && (DateTime.UtcNow - s.CreatedOn < TimeSpan.FromMinutes(1)));
  }
}
