using Bogus;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RenewSessionCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly RenewSessionCommandHandler _handler;

  private readonly UserAggregate _user;

  public RenewSessionCommandHandlerTests()
  {
    _handler = new(_passwordManager.Object, _sessionQuerier.Object, _sessionRepository.Object);

    _user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
  }

  [Fact(DisplayName = "It should throw IncorrectSessionSecretException when the secret is not correct.")]
  public async Task It_should_throw_IncorrectSessionSecretException_when_the_secret_is_not_correct()
  {
    string correctSecret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    SessionAggregate session = new(_user, new PasswordMock(correctSecret));
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    string incorrectSecret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, incorrectSecret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectSessionSecretException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(session.Id, exception.SessionId);
    Assert.Equal(incorrectSecret, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "It should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task It_should_throw_InvalidRefreshTokenException_when_the_refresh_token_is_not_valid()
  {
    RenewSessionPayload payload = new("invalid");
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SessionIsNotActiveException when the session is not active.")]
  public async Task It_should_throw_SessionIsNotActiveException_when_the_session_is_not_active()
  {
    SessionAggregate session = new(_user);
    session.SignOut();
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, secret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionIsNotActiveException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(session.Id, exception.SessionId);
  }

  [Fact(DisplayName = "It should throw SessionIsNotPersistentException when the session is not persistent.")]
  public async Task It_should_throw_SessionIsNotPersistentException_when_the_session_is_not_persistent()
  {
    SessionAggregate session = new(_user);
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, secret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionIsNotPersistentException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(session.Id, exception.SessionId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    RenewSessionPayload payload = new();
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("RefreshToken", error.PropertyName);
  }
}
