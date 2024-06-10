using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutSessionCommandTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the session could not be found.")]
  public async Task It_should_return_null_when_the_session_could_not_be_found()
  {
    SignOutSessionCommand command = new(Id: Guid.NewGuid());
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should sign-out the specified session.")]
  public async Task It_should_sign_out_the_specified_session()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate aggregate = new(user);
    await _sessionRepository.SaveAsync(aggregate);

    SignOutSessionCommand command = new(aggregate.Id.ToGuid());
    Session? session = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(session);

    Assert.Equal(aggregate.Id.ToGuid(), session.Id);
    Assert.Equal(aggregate.Version + 1, session.Version);
    Assert.Equal(Actor, session.CreatedBy);
    Assert.Equal(Actor, session.UpdatedBy);
    Assert.True(session.CreatedOn < session.UpdatedOn);

    Assert.False(session.IsActive);
    Assert.Equal(Actor, session.SignedOutBy);
    Assert.NotNull(session.SignedOutOn);

    Assert.Equal(user.Id.ToGuid(), session.User.Id);
  }
}
