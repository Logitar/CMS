using Bogus;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using Logitar.Identity.Core.Users.Events;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignInSessionCommandHandlerTests
{
  private const string PasswordString = "Test123!";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserManager> _userManager = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignInSessionCommandHandler _handler;

  public SignInSessionCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _passwordManager.Object, _sessionQuerier.Object, _sessionRepository.Object, _userManager.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "Handle: it should create a persistent session.")]
  public async Task Given_Persistent_When_Handle_Then_PersistentSessionCreated()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    user.SetEmail(new Email(_faker.Person.Email, isVerified: true));
    user.SetPassword(new Base64Password(PasswordString));
    Assert.NotNull(user.Email);
    FoundUsers users = new()
    {
      ByEmail = user
    };
    _userManager.Setup(x => x.FindAsync(null, user.Email.Address, _cancellationToken)).ReturnsAsync(users);

    ActorId actorId = new("SYSTEM");
    _applicationContext.Setup(x => x.ActorId).Returns(actorId);

    SessionModel model = new()
    {
      Id = Guid.NewGuid()
    };
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<Session>(), _cancellationToken)).ReturnsAsync(model);

    string secretString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshToken.SecretLength));
    Base64Password secret = new(secretString);
    _passwordManager.Setup(x => x.GenerateBase64(RefreshToken.SecretLength, out secretString)).Returns(secret);

    SignInSessionPayload payload = new(user.Email.Address, PasswordString, model.Id, isPersistent: true);
    SignInSessionCommand command = new(payload);
    SessionModel session = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, session);

    Assert.NotNull(model.RefreshToken);
    Assert.Equal(model.RefreshToken, new RefreshToken(model.Id, secretString).Encode());

    Assert.Contains(user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId == actorId);

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);

    _sessionRepository.Verify(x => x.SaveAsync(
      It.Is<Session>(s => s.CreatedBy == actorId && s.TenantId == null && s.EntityId.ToGuid() == model.Id && s.UserId == user.Id && s.IsPersistent && s.IsActive && s.CustomAttributes.Count == 0),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should create an ephemeral session.")]
  public async Task Given_NotPersistent_When_Handle_Then_EphemeralSessionCreated()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    user.SetPassword(new Base64Password(PasswordString));
    FoundUsers users = new()
    {
      ByUniqueName = user
    };
    _userManager.Setup(x => x.FindAsync(null, user.UniqueName.Value, _cancellationToken)).ReturnsAsync(users);

    SessionModel model = new();
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<Session>(), _cancellationToken)).ReturnsAsync(model);

    SignInSessionPayload payload = new(user.UniqueName.Value, PasswordString);
    payload.CustomAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    SignInSessionCommand command = new(payload);
    SessionModel session = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, session);

    ActorId actorId = new(user.Id.Value);
    Assert.Contains(user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId == actorId);

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);

    _sessionRepository.Verify(x => x.SaveAsync(
      It.Is<Session>(s => s.CreatedBy == actorId && s.TenantId == null && s.UserId == user.Id && !s.IsPersistent && s.IsActive
        && s.CustomAttributes.Count == payload.CustomAttributes.Count && payload.CustomAttributes.All(c => s.CustomAttributes[new Identifier(c.Key)] == c.Value)),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should throw TooManyResultsException when multiple users were found.")]
  public async Task Given_MultipleUsersFound_When_Handle_Then_TooManyResultsException()
  {
    string uniqueName = _faker.Person.Email;
    UniqueNameSettings uniqueNameSettings = new();
    FoundUsers users = new()
    {
      ByUniqueName = new(new UniqueName(uniqueNameSettings, uniqueName)),
      ByEmail = new(new UniqueName(uniqueNameSettings, _faker.Internet.UserName()))
    };
    users.ByEmail.SetEmail(new Email(uniqueName, isVerified: true));
    _userManager.Setup(x => x.FindAsync(null, uniqueName, _cancellationToken)).ReturnsAsync(users);

    SignInSessionPayload payload = new(uniqueName, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "Handle: it should throw UserNotFoundException when no user was found.")]
  public async Task Given_NoUserFound_When_Handle_Then_UserNotFoundException()
  {
    string uniqueName = _faker.Person.UserName;
    FoundUsers users = new();
    _userManager.Setup(x => x.FindAsync(null, uniqueName, _cancellationToken)).ReturnsAsync(users);

    SignInSessionPayload payload = new(uniqueName, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "Handle: it should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    SignInSessionPayload payload = new()
    {
      UniqueName = string.Empty,
      Password = "    ",
      CustomAttributes = [new CustomAttribute("123_Test", value: string.Empty)]
    };
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "CustomAttributes[0].Value");
  }
}
