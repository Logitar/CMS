using Bogus;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users.Events;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateUserHandlerTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly ConfigurationAggregate _configuration;

  private readonly UpdateUserHandler _handler;

  public UpdateUserHandlerTests()
  {
    _configuration = new(actorId: _actorId);

    _applicationContext.SetupGet(x => x.ActorId)
      .Returns(_actorId);
    _applicationContext.SetupGet(x => x.Configuration)
      .Returns(_configuration);

    _handler = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact]
  public async Task When_email_is_null_Then_it_is_removed()
  {
    UserAggregate user = new(_actorId, _configuration, _faker.Person.UserName);
    Guid id = user.Id.ToGuid();
    _userRepository.Setup(x => x.LoadAsync(id, _cancellationToken))
      .ReturnsAsync(user);

    user.SetEmail(new ReadOnlyEmail(_faker.Person.Email));
    Assert.NotNull(user.Email);

    UpdateUserInput input = new() { Email = null };
    _ = await _handler.Handle(new UpdateUser(id, input), _cancellationToken);

    Assert.Null(user.Email);

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_password_is_null_Then_it_is_not_changed()
  {
    UserAggregate user = new(_actorId, _configuration, _faker.Person.UserName);
    Guid id = user.Id.ToGuid();
    _userRepository.Setup(x => x.LoadAsync(id, _cancellationToken))
      .ReturnsAsync(user);

    user.ChangePassword(_configuration, "P@s$W0rD");
    Assert.True(user.HasPassword);
    int count = user.Changes.Where(e => e is PasswordChanged).Count();

    UpdateUserInput input = new() { Password = null };
    _ = await _handler.Handle(new UpdateUser(id, input), _cancellationToken);

    Assert.True(user.HasPassword);

    _userRepository.Verify(x => x.SaveAsync(It.Is<UserAggregate>(y => y.Equals(user)
      && user.Changes.Where(e => e is PasswordChanged).Count() == count), _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_user_is_found_Then_it_is_updated()
  {
    UserAggregate user = new(_actorId, _configuration, _faker.Person.UserName);
    Guid id = user.Id.ToGuid();
    _userRepository.Setup(x => x.LoadAsync(id, _cancellationToken))
      .ReturnsAsync(user);
    Assert.False(user.HasPassword);

    User expected = new();
    _userQuerier.Setup(x => x.GetAsync(user, _cancellationToken))
      .ReturnsAsync(expected);

    UpdateUserInput input = new()
    {
      Password = "P@s$W0rD",
      Email = new EmailInput
      {
        Address = _faker.Person.Email,
        Verify = true
      },
      FirstName = _faker.Person.FirstName,
      LastName = _faker.Person.LastName,
      Locale = "fr-CA",
      Picture = "https://www.test.com/assets/img/admin.jpg"
    };
    User actual = await _handler.Handle(new UpdateUser(id, input), _cancellationToken);
    Assert.Same(expected, actual);

    Assert.True(user.HasPassword);
    Assert.Equal(input.Email.Address, user.Email?.Address);
    Assert.Equal(input.Email.Verify, user.Email?.IsVerified);
    Assert.Equal(input.FirstName, user.FirstName);
    Assert.Equal(input.LastName, user.LastName);
    Assert.Equal(_faker.Person.FullName, user.FullName);
    Assert.Equal(input.Locale, user.Locale?.Name);
    Assert.Equal(input.Picture, user.Picture?.ToString());

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_user_is_not_found_Then_AggregateNotFoundException_is_thrown()
  {
    await Assert.ThrowsAsync<AggregateNotFoundException<UserAggregate>>(async ()
      => await _handler.Handle(new UpdateUser(Guid.NewGuid(), new UpdateUserInput()), _cancellationToken));
  }
}
