using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Moq;
using System.Security.Cryptography;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RefreshHandlerTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly RefreshHandler _handler;

  public RefreshHandlerTests()
  {
    _handler = new(_sessionQuerier.Object, _sessionRepository.Object);
  }

  [Fact]
  public async Task When_session_could_not_be_found_Then_InvalidCredentialsException_is_thrown()
  {
    Guid id = Guid.NewGuid();
    byte[] secret = RandomNumberGenerator.GetBytes(32);
    RefreshInput input = new() { RefreshToken = new RefreshToken(id, secret).ToString() };

    var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(async ()
      => await _handler.Handle(new Refresh(input), _cancellationToken));
    Assert.Equal($"The session aggregate '{id}' could not be found.", exception.Message);
  }

  [Fact]
  public async Task When_using_a_valid_refresh_token_Then_session_is_refreshed()
  {
    ConfigurationAggregate configuration = new(actorId: _actorId);
    UserAggregate user = new(_actorId, configuration, "admin");
    SessionAggregate session = new(user, isPersistent: true);
    Assert.True(session.RefreshToken.HasValue);
    _sessionRepository.Setup(x => x.LoadAsync(session.Id.ToGuid(), _cancellationToken))
      .ReturnsAsync(session);

    Session expected = new();
    _sessionQuerier.Setup(x => x.GetAsync(session, _cancellationToken))
      .ReturnsAsync(expected);

    RefreshInput input = new()
    {
      RefreshToken = session.RefreshToken.Value.ToString(),
      IpAddress = "::1",
      AdditionalInformation = "{\"User-Agent\":\"Chrome\"}"
    };
    Session actual = await _handler.Handle(new Refresh(input), _cancellationToken);
    Assert.Same(expected, actual);
    Assert.True(session.RefreshToken.HasValue);
    Assert.NotEqual(input.RefreshToken, session.RefreshToken.Value.ToString());

    _sessionRepository.Verify(x => x.SaveAsync(session, _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_using_an_invalid_refresh_token_Then_InvalidCredentialsException_is_thrown()
  {
    RefreshInput input = new() { RefreshToken = "test" };

    var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(async ()
      => await _handler.Handle(new Refresh(input), _cancellationToken));
    Assert.Equal($"The value '{input.RefreshToken}' is not a valid refresh token.", exception.Message);
  }
}
