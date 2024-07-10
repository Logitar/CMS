using Bogus;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Moq;

namespace Logitar.Cms.Core.Users.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class FindUserQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICacheService> _cacheService = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly FindUserQueryHandler _handler;

  public FindUserQueryHandlerTests()
  {
    _handler = new(_cacheService.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should return the user found by email address.")]
  public async Task It_should_return_the_user_found_by_email_address()
  {
    _cacheService.SetupGet(x => x.Configuration).Returns(new Configuration
    {
      RequireUniqueEmail = true
    });

    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
    user.SetEmail(new EmailUnit(_faker.Person.Email));
    Assert.NotNull(user.Email);
    _userRepository.Setup(x => x.LoadAsync(null, user.Email, _cancellationToken)).ReturnsAsync([user]);

    FindUserQuery query = new(user.Email.Address, PropertyName: null);
    UserAggregate result = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(user, result);
  }

  [Fact(DisplayName = "It should return the user found by unique name.")]
  public async Task It_should_return_the_user_found_by_unique_name()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
    _userRepository.Setup(x => x.LoadAsync(null, user.UniqueName, _cancellationToken)).ReturnsAsync(user);

    FindUserQuery query = new(user.UniqueName.Value, PropertyName: null);
    UserAggregate result = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(user, result);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user cannot be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_cannot_be_found()
  {
    _cacheService.SetupGet(x => x.Configuration).Returns(new Configuration
    {
      RequireUniqueEmail = false
    });

    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
    user.SetEmail(new EmailUnit(_faker.Person.Email));
    Assert.NotNull(user.Email);

    FindUserQuery query = new(user.Email.Address, "EmailAddress");
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.UniqueName, exception.UniqueName);
    Assert.Equal(query.PropertyName, exception.PropertyName);
  }
}
