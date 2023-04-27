using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Sessions.Events;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutHandlerTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly SignOutHandler _handler;

  public SignOutHandlerTests()
  {
    _applicationContext.SetupGet(x => x.ActorId)
      .Returns(_actorId);

    _handler = new(_applicationContext.Object, _sessionQuerier.Object, _sessionRepository.Object);
  }

  [Fact]
  public async Task When_session_is_found_Then_it_is_signed_out()
  {
    ConfigurationAggregate configuration = new(actorId: _actorId);
    UserAggregate user = new(_actorId, configuration, "admin");
    SessionAggregate session = new(user);
    Guid id = session.Id.ToGuid();
    _sessionRepository.Setup(x => x.LoadAsync(id, _cancellationToken))
      .ReturnsAsync(session);

    Session expected = new();
    _sessionQuerier.Setup(x => x.GetAsync(session, _cancellationToken))
      .ReturnsAsync(expected);

    Session actual = await _handler.Handle(new SignOut(id), _cancellationToken);
    Assert.Same(expected, actual);

    Assert.False(session.IsActive);
    Assert.Equal(_actorId, session.Changes.Single(e => e is SessionSignedOut).ActorId);

    _sessionRepository.Verify(x => x.SaveAsync(session, _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_session_is_not_found_Then_AggregateNotFoundException_is_thrown()
  {
    await Assert.ThrowsAsync<AggregateNotFoundException<SessionAggregate>>
      (async () => await _handler.Handle(new SignOut(Guid.NewGuid()), _cancellationToken));
  }
}
