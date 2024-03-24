using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadSessionQueryTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public ReadSessionQueryTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the session is not found.")]
  public async Task It_should_return_null_when_the_session_is_not_found()
  {
    ReadSessionQuery query = new(Id: Guid.NewGuid());
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the session found by ID.")]
  public async Task It_should_return_the_session_found_by_Id()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate aggregate = new(user);
    await _sessionRepository.SaveAsync(aggregate);

    ReadSessionQuery query = new(aggregate.Id.ToGuid());
    Session? session = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(session);
    Assert.Equal(aggregate.Id.ToGuid(), session.Id);
  }
}
