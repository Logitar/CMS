using FluentValidation.Results;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class RenewSessionCommandTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public RenewSessionCommandTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should renew the specified session.")]
  public async Task It_should_renew_the_specified_session()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    Password secret = _passwordManager.GenerateBase64(32, out string secretString);
    SessionAggregate aggregate = user.SignIn(secret);
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(aggregate);

    RenewSessionPayload payload = new(RefreshToken.Encode(aggregate, secretString))
    {
      IpAddress = Faker.Internet.Ip(),
      AdditionalInformation = $@"  {{""User-Agent"":""{Faker.Internet.UserAgent()}""}}  "
    };
    RenewSessionCommand command = new(payload);
    Session session = await Pipeline.ExecuteAsync(command);

    Assert.Equal(aggregate.Id.ToGuid(), session.Id);
    Assert.Equal(aggregate.Version + 2, session.Version);
    Assert.Equal(Actor, session.CreatedBy);
    Assert.Equal(Actor, session.UpdatedBy);
    Assert.True(session.CreatedOn < session.UpdatedOn);

    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.IpAddress.Trim(), session.IpAddress);
    Assert.Equal(payload.AdditionalInformation.Trim(), session.AdditionalInformation);
    Assert.Equal(user.Id.ToGuid(), session.User.Id);

    RefreshToken refreshToken = RefreshToken.Decode(session.RefreshToken);
    Assert.Equal(aggregate.Id, refreshToken.Id);
    Assert.Equal(32, Convert.FromBase64String(refreshToken.Secret).Length);
  }

  [Fact(DisplayName = "It should throw IncorrectSessionSecretException when the secret is not correct.")]
  public async Task It_should_throw_IncorrectSessionSecretException_when_the_secret_is_not_correct()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    Password secret = _passwordManager.GenerateBase64(32, out string secretString);
    SessionAggregate session = new(user, secret);
    await _sessionRepository.SaveAsync(session);

    string incorrectSecret = RandomStringGenerator.GetBase64String(32, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, incorrectSecret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectSessionSecretException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(session.Id, exception.SessionId);
    Assert.Equal(incorrectSecret, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "It should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task It_should_throw_InvalidRefreshTokenException_when_the_refresh_token_is_not_valid()
  {
    RenewSessionPayload payload = new("PT.ABC.123");
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
    Assert.Equal(nameof(payload.RefreshToken), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SessionIsNotActiveException when the session is not active.")]
  public async Task It_should_throw_SessionIsNotActiveException_when_the_session_is_not_active()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate session = new(user);
    session.SignOut(ActorId);
    await _sessionRepository.SaveAsync(session);

    string secret = RandomStringGenerator.GetBase64String(32, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, secret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionIsNotActiveException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(session.Id, exception.SessionId);
  }

  [Fact(DisplayName = "It should throw SessionIsNotPersistentException when the session is not persistent.")]
  public async Task It_should_throw_SessionIsNotPersistentException_when_the_session_is_not_persistent()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate session = new(user);
    await _sessionRepository.SaveAsync(session);

    string secret = RandomStringGenerator.GetBase64String(32, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, secret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionIsNotPersistentException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(session.Id, exception.SessionId);
  }

  [Fact(DisplayName = "It should throw SessionNotFoundException when the session could not be found.")]
  public async Task It_should_throw_SessionNotFoundException_when_the_session_could_not_be_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate session = new(user);

    string secret = RandomStringGenerator.GetBase64String(32, out _);
    RenewSessionPayload payload = new(RefreshToken.Encode(session, secret));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(session.Id.Value, exception.Id);
    Assert.Equal(nameof(payload.RefreshToken), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    RenewSessionPayload payload = new();
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.RefreshToken), error.PropertyName);
  }
}
