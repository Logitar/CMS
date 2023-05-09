using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignInHandlerTests
{
  private const string Username = "admin";

  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignInHandler _handler;

  public SignInHandlerTests()
  {
    _handler = new(_sessionQuerier.Object, _sessionRepository.Object, _userRepository.Object);
  }

  [Fact]
  public async Task When_user_signs_in_Then_a_session_is_issued()
  {
    SignInInput input = new()
    {
      Username = Username,
      Password = "P@s$W0rD"
    };

    ConfigurationAggregate configuration = new(actorId: _actorId);
    UserAggregate user = new(_actorId, configuration, Username);
    user.ChangePassword(configuration, input.Password);
    _userRepository.Setup(x => x.LoadAsync(Username, _cancellationToken))
      .ReturnsAsync(user);

    Session session = new();
    _sessionQuerier.Setup(x => x.GetAsync(It.Is<SessionAggregate>(y => y.UserId == user.Id), _cancellationToken))
      .ReturnsAsync(session);

    Session output = await _handler.Handle(new SignIn(input), _cancellationToken);
    Assert.Same(session, output);

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);

    _sessionRepository.Verify(x => x.SaveAsync(It.Is<SessionAggregate>(y => y.UserId == user.Id), _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_username_is_not_found_Then_InvalidCredentialsException_is_thrown()
  {
    SignIn command = new(new SignInInput { Username = Username });
    var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal($"The user '{Username}' could not be found.", exception.Message);
  }
}
