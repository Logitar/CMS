using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutUserCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignOutUserCommandHandler _handler;

  public SignOutUserCommandHandlerTests()
  {
    _handler = new(_sessionRepository.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the user could not be found.")]
  public async Task It_should_return_null_when_the_user_could_not_be_found()
  {
    SignOutUserCommand command = new(Guid.NewGuid());
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}
