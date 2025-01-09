using Bogus;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Sessions.Events;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutSessionCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly SignOutSessionCommandHandler _handler;

  public SignOutSessionCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _sessionQuerier.Object, _sessionRepository.Object);
  }

  [Fact(DisplayName = "Handle: it should return null when the session was not found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    SignOutSessionCommand command = new(Guid.NewGuid());
    SessionModel? session = await _handler.Handle(command, _cancellationToken);
    Assert.Null(session);
  }

  [Fact(DisplayName = "Handle: it should sign-out the session found.")]
  public async Task Given_SessionFound_When_Handle_Then_SignedOut()
  {
    ActorId actorId = new("SYSTEM");
    _applicationContext.Setup(x => x.ActorId).Returns(actorId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    Session session = new(user);
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    SessionModel model = new();
    _sessionQuerier.Setup(x => x.ReadAsync(session, _cancellationToken)).ReturnsAsync(model);

    SignOutSessionCommand command = new(session.EntityId.ToGuid());
    SessionModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Equal(model, result);

    Assert.False(session.IsActive);
    Assert.Contains(session.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == actorId);

    _sessionRepository.Verify(x => x.SaveAsync(session, _cancellationToken), Times.Once);
  }
}
