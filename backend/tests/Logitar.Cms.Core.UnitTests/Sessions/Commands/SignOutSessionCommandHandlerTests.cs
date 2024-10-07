using Logitar.Identity.Domain.Sessions;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutSessionCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly SignOutSessionCommandHandler _handler;

  public SignOutSessionCommandHandlerTests()
  {
    _handler = new(_sessionQuerier.Object, _sessionRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the session could not be found.")]
  public async Task It_should_return_null_when_the_session_could_not_be_found()
  {
    SignOutSessionCommand command = new(Guid.NewGuid());
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}
