using Bogus;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users.Events;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ChangePasswordHandlerTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly ConfigurationAggregate _configuration;

  private readonly ChangePasswordHandler _handler;

  public ChangePasswordHandlerTests()
  {
    _configuration = new(actorId: _actorId);

    _applicationContext.SetupGet(x => x.ActorId)
      .Returns(_actorId);
    _applicationContext.SetupGet(x => x.Configuration)
      .Returns(_configuration);

    _handler = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact]
  public async Task When_user_is_found_Then_its_password_is_changed()
  {
    UserAggregate user = new(_actorId, _configuration, _faker.Person.UserName);
    Guid id = user.Id.ToGuid();
    _userRepository.Setup(x => x.LoadAsync(id, _cancellationToken))
      .ReturnsAsync(user);

    ChangePasswordInput input = new()
    {
      Current = "Test123!",
      Password = "P@s$W0rD"
    };
    user.ChangePassword(_configuration, input.Current);

    User expected = new();
    _userQuerier.Setup(x => x.GetAsync(user, _cancellationToken))
      .ReturnsAsync(expected);

    User actual = await _handler.Handle(new ChangePassword(id, input), _cancellationToken);
    Assert.Same(expected, actual);

    _userRepository.Verify(x => x.SaveAsync(It.Is<UserAggregate>(y => y.Equals(user)
      && y.Changes.Where(e => e is PasswordChanged).Count() == 2), _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_user_is_not_found_Then_AggregateNotFoundException_is_thrown()
  {
    await Assert.ThrowsAsync<AggregateNotFoundException<UserAggregate>>(async ()
      => await _handler.Handle(new ChangePassword(Guid.NewGuid(), new ChangePasswordInput()), _cancellationToken));
  }
}
