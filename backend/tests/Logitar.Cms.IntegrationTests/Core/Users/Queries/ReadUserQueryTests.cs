using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Shared;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Users.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadUserQueryTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public ReadUserQueryTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the user is not found.")]
  public async Task It_should_return_null_when_the_user_is_not_found()
  {
    ReadUserQuery query = new(Id: Guid.NewGuid(), Username: Faker.Internet.UserName());
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the user found by ID.")]
  public async Task It_should_return_the_user_found_by_Id()
  {
    UserAggregate aggregate = Assert.Single(await _userRepository.LoadAsync());

    ReadUserQuery query = new(aggregate.Id.ToGuid(), Username: null);
    User? user = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(user);
    Assert.Equal(aggregate.Id.ToGuid(), user.Id);
  }

  [Fact(DisplayName = "It should return the user found by email address.")]
  public async Task It_should_return_the_user_found_by_email_address()
  {
    UserAggregate aggregate = Assert.Single(await _userRepository.LoadAsync());
    Assert.NotNull(aggregate.Email);

    ReadUserQuery query = new(Id: null, aggregate.Email.Address);
    User? user = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(user);
    Assert.Equal(aggregate.Id.ToGuid(), user.Id);
  }

  [Fact(DisplayName = "It should return the user found by username.")]
  public async Task It_should_return_the_user_found_by_username()
  {
    UserAggregate aggregate = Assert.Single(await _userRepository.LoadAsync());

    ReadUserQuery query = new(Id: null, aggregate.UniqueName.Value);
    User? user = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(user);
    Assert.Equal(aggregate.Id.ToGuid(), user.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many users are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_users_are_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    UserAggregate otherUser = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    otherUser.SetEmail(new EmailUnit(Faker.Internet.Email()));
    await _userRepository.SaveAsync(otherUser);
    Assert.NotNull(otherUser.Email);

    ReadUserQuery query = new(user.Id.ToGuid(), otherUser.Email.Address);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(async () => await Pipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
