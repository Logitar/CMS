using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutSessionCommandHandlerTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandHandlerTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should sign-out an active session.")]
  public async Task It_should_sign_out_an_active_session()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate session = new(user);
    await _sessionRepository.SaveAsync(session);

    SignOutSessionCommand command = new(session.Id.ToGuid());
    Session? result = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(result);

    Assert.Equal(session.Id.ToGuid(), result.Id);
    Assert.Equal(session.Version + 1, result.Version);
    Assert.Equal(user.Id.ToGuid(), result.CreatedBy.Id);
    Assert.Equal(session.CreatedOn.AsUniversalTime(), result.CreatedOn);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.False(result.IsActive);
    Assert.Equal(Actor, result.SignedOutBy);
    Assert.Equal(result.UpdatedOn, result.SignedOutOn);
  }
}
