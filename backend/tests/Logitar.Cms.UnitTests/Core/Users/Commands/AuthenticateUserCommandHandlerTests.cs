using Bogus;
using Logitar.Cms.Core.Users.Models;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class AuthenticateUserCommandHandlerTests
{
  private const string PasswordString = "Test123!";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IUserManager> _userManager = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly AuthenticateUserCommandHandler _handler;

  public AuthenticateUserCommandHandlerTests()
  {
    _handler = new(_userManager.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "Handle: it should authenticate the user when the credentials are valid.")]
  public async Task Given_ValidCredentials_When_Handle_Then_UserAuthenticated()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    user.SetPassword(new Base64Password(PasswordString));

    AuthenticateUserPayload payload = new(user.UniqueName.Value, PasswordString);

    FoundUsers foundUsers = new()
    {
      ByUniqueName = user
    };
    _userManager.Setup(x => x.FindAsync(null, payload.UniqueName, _cancellationToken)).ReturnsAsync(foundUsers);

    UserModel model = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(model);

    AuthenticateUserCommand command = new(payload);
    UserModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result);

    Assert.True(user.AuthenticatedOn.HasValue);
    Assert.Equal(user.AuthenticatedOn.Value, DateTime.Now, TimeSpan.FromSeconds(1));

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should throw TooManyResultsException when many users were found.")]
  public async Task Given_MultipleUsersFound_When_Handle_Then_TooManyResultsException()
  {
    AuthenticateUserPayload payload = new(_faker.Person.UserName, PasswordString);
    UniqueNameSettings uniqueNameSettings = new();
    FoundUsers foundUsers = new()
    {
      ByUniqueName = new User(new UniqueName(uniqueNameSettings, _faker.Person.UserName)),
      ByEmail = new User(new UniqueName(uniqueNameSettings, _faker.Internet.UserName()))
    };
    _userManager.Setup(x => x.FindAsync(null, payload.UniqueName, _cancellationToken)).ReturnsAsync(foundUsers);

    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "Handle: it should throw UserNotFoundException when no user was found.")]
  public async Task Given_NoUserFound_When_Handle_Then_UserNotFoundException()
  {
    AuthenticateUserPayload payload = new(_faker.Person.UserName, PasswordString);
    FoundUsers foundUsers = new();
    _userManager.Setup(x => x.FindAsync(null, payload.UniqueName, _cancellationToken)).ReturnsAsync(foundUsers);

    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "Handle: it should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    AuthenticateUserPayload payload = new();
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
  }
}
