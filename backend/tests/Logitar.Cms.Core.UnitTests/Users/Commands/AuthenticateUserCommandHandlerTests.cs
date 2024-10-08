using Bogus;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Builders;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.Domain.Users.Events;
using Moq;
using ValidationException = FluentValidation.ValidationException;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class AuthenticateUserCommandHandlerTests
{
  private const string PasswordString = "Test123!";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IUserManager> _userManager = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();

  private readonly AuthenticateUserCommandHandler _handler;

  public AuthenticateUserCommandHandlerTests()
  {
    _handler = new(_userManager.Object, _userQuerier.Object);
  }

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task It_should_authenticate_the_user()
  {
    UserAggregate user = new UserBuilder(_faker).WithPassword(PasswordString).Build();
    FoundUsers foundUsers = new()
    {
      ByUniqueName = user
    };
    _userManager.Setup(x => x.FindAsync(null, user.UniqueName.Value, It.IsAny<IUserSettings>(), _cancellationToken)).ReturnsAsync(foundUsers);

    UserModel model = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(model);

    AuthenticateUserPayload payload = new(user.UniqueName.Value, PasswordString);
    AuthenticateUserCommand command = new(payload);

    UserModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result);

    Assert.Contains(user.Changes, change => change is UserAuthenticatedEvent @event && @event.ActorId.Value == user.Id.Value);

    _userManager.Verify(x => x.SaveAsync(user, It.IsAny<IUserSettings>(), It.Is<ActorId>(y => y.Value == user.Id.Value), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateUserPayload payload = new();
    AuthenticateUserCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Username");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
  }
}
