using Bogus;
using Logitar.Cms.Core.Users.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Sessions.Events;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutUserCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignOutUserCommandHandler _handler;

  public SignOutUserCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _sessionRepository.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "Handle: it should return null when the user could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    SignOutUserCommand command = new(Guid.NewGuid());
    UserModel? user = await _handler.Handle(command, _cancellationToken);
    Assert.Null(user);
  }

  [Fact(DisplayName = "Handle: it should sign-out the active user sessions.")]
  public async Task Given_UserFound_When_Handle_Then_ActiveSessionsSignedOut()
  {
    ActorId actorId = new("SYSTEM");
    _applicationContext.Setup(x => x.ActorId).Returns(actorId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    Session[] sessions = [new(user), new(user)];
    _sessionRepository.Setup(x => x.LoadActiveAsync(user, _cancellationToken)).ReturnsAsync(sessions);

    UserModel model = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(model);

    SignOutUserCommand command = new(user.EntityId.ToGuid());
    UserModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _sessionRepository.Verify(x => x.SaveAsync(sessions, _cancellationToken), Times.Once);

    foreach (Session session in sessions)
    {
      Assert.False(session.IsActive);
      Assert.Contains(session.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == actorId);
    }
  }
}
