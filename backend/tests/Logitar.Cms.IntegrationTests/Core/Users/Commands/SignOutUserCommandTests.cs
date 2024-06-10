using Logitar.Cms.Contracts.Users;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutUserCommandTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the user could not be found.")]
  public async Task It_should_return_null_when_the_user_could_not_be_found()
  {
    SignOutUserCommand command = new(Id: Guid.NewGuid());
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should sign-out the specified user.")]
  public async Task It_should_sign_out_the_specified_user()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SessionAggregate session = new(user);
    await _sessionRepository.SaveAsync(session);

    SignOutUserCommand command = new(Actor.Id);
    User? result = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Equal(user.Id.ToGuid(), result.Id);

    SessionEntity[] sessions = await IdentityContext.Sessions.AsNoTracking()
      .Include(x => x.User)
      .Where(x => x.User!.AggregateId == user.Id.Value).ToArrayAsync();
    Assert.NotEmpty(sessions);
    Assert.All(sessions, session =>
    {
      Assert.Equal(ActorId.Value, session.SignedOutBy);
      Assert.NotNull(session.SignedOutOn);
      Assert.False(session.IsActive);
    });
  }
}
