using Bogus;
using FluentValidation.Results;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users.Queries;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignInSessionCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignInSessionCommandHandler _handler;

  public SignInSessionCommandHandlerTests()
  {
    _handler = new(_passwordManager.Object, _sender.Object, _sessionQuerier.Object, _sessionRepository.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should create an ephemereal session.")]
  public async Task It_should_create_an_ephemereal_session()
  {
    const string password = "Test123!";

    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
    user.SetPassword(new PasswordMock(password));
    _sender.Setup(x => x.Send(It.Is<FindUserQuery>(q => q.UniqueName == user.UniqueName.Value && q.PropertyName == "UniqueName"),
      _cancellationToken)).ReturnsAsync(user);

    Session session = new();
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<SessionAggregate>(), _cancellationToken)).ReturnsAsync(session);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""""User-Agent"""":""""{_faker.Internet.UserAgent()}""""}}"),
      new("IpAddress", _faker.Internet.Ip())
    ];
    SignInSessionPayload payload = new(user.UniqueName.Value, password, isPersistent: false, customAttributes);
    SignInSessionCommand command = new(payload);
    Session result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(session, result);

    _sessionRepository.Verify(x => x.SaveAsync(It.Is<SessionAggregate>(s => s.UserId == user.Id
      && !s.IsPersistent && s.IsActive
      && s.CustomAttributes.Count == 2 && customAttributes.All(c => s.CustomAttributes[c.Key] == c.Value)
    ), _cancellationToken), Times.Once);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
    user.SetPassword(new PasswordMock("Test123!"));
    _sender.Setup(x => x.Send(It.Is<FindUserQuery>(q => q.UniqueName == user.UniqueName.Value && q.PropertyName == "UniqueName"),
      _cancellationToken)).ReturnsAsync(user);

    SignInSessionPayload payload = new(user.UniqueName.Value, "AAaa!!11");
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SignInSessionPayload payload = new("admin", string.Empty);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Password", error.PropertyName);
  }
}
