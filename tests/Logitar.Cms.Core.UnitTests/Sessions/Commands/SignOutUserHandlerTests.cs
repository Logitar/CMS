using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Sessions.Events;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutUserHandlerTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignOutUserHandler _handler;

  public SignOutUserHandlerTests()
  {
    _applicationContext.SetupGet(x => x.ActorId)
      .Returns(_actorId);

    _handler = new(_applicationContext.Object, _sessionQuerier.Object, _sessionRepository.Object, _userRepository.Object);
  }

  [Fact]
  public async Task When_user_is_found_Then_its_active_sessions_are_signed_out()
  {
    ConfigurationAggregate configuration = new(actorId: _actorId);
    UserAggregate user = new(_actorId, configuration, "admin");
    Guid id = user.Id.ToGuid();
    _userRepository.Setup(x => x.LoadAsync(id, _cancellationToken))
      .ReturnsAsync(user);

    var sessions = new[]
    {
      new SessionAggregate(user),
      new SessionAggregate(user, isPersistent: true)
    };
    _sessionRepository.Setup(x => x.LoadActiveAsync(user, _cancellationToken))
      .ReturnsAsync(sessions);

    IEnumerable<Session> expected = sessions.Select(s => new Session
    {
      Id = s.Id.ToGuid()
    });
    _sessionQuerier.Setup(x => x.GetAsync(sessions, _cancellationToken))
      .ReturnsAsync(expected);

    IEnumerable<Session> actual = await _handler.Handle(new SignOutUser(id), _cancellationToken);
    Assert.Same(expected, actual);

    foreach (SessionAggregate session in sessions)
    {
      Assert.False(session.IsActive);
      Assert.Equal(_actorId, session.Changes.Single(e => e is SessionSignedOut).ActorId);
    }

    _sessionRepository.Verify(x => x.SaveAsync(sessions, _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_user_is_not_found_Then_AggregateNotFoundException_is_thrown()
  {
    await Assert.ThrowsAsync<AggregateNotFoundException<UserAggregate>>
      (async () => await _handler.Handle(new SignOutUser(Guid.NewGuid()), _cancellationToken));
  }
}
