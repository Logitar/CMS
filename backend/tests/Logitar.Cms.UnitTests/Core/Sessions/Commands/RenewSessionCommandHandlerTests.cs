using Bogus;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Sessions.Events;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using Moq;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RenewSessionCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly RenewSessionCommandHandler _handler;

  public RenewSessionCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _passwordManager.Object, _sessionQuerier.Object, _sessionRepository.Object);
  }

  [Fact(DisplayName = "Handle: it should renew the session found given valid credentials.")]
  public async Task Given_ValidCredentials_When_Handle_Then_SessionRenewed()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));

    string secretString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshToken.SecretLength));
    Base64Password secret = new(secretString);
    _passwordManager.Setup(x => x.GenerateBase64(RefreshToken.SecretLength, out secretString)).Returns(secret);

    Session session = user.SignIn(secret);
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    ActorId actorId = new("SYSTEM");
    _applicationContext.Setup(x => x.ActorId).Returns(actorId);

    SessionModel model = new();
    _sessionQuerier.Setup(x => x.ReadAsync(session, _cancellationToken)).ReturnsAsync(model);

    RefreshToken refreshToken = new(session.EntityId.ToGuid(), secretString);
    RenewSessionPayload payload = new(refreshToken.Encode(),
    [
      new CustomAttribute("IpAddress", _faker.Internet.Ip()),
      new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}")
    ]);
    RenewSessionCommand command = new(payload);
    SessionModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result);

    Assert.Contains(session.Changes, change => change is SessionRenewed renewed && renewed.ActorId == actorId && renewed.Secret.Equals(secret));

    Assert.Equal(payload.CustomAttributes.Count, session.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, session.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    _sessionRepository.Verify(x => x.SaveAsync(session, _cancellationToken), Times.Once);

    Assert.NotNull(result.RefreshToken);
    Assert.Equal(refreshToken.Encode(), result.RefreshToken);
  }

  [Fact(DisplayName = "Handle: it should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task Given_InvalidRefreshToken_When_Handle_Then_InvalidRefreshTokenException()
  {
    RenewSessionPayload payload = new("invalid");
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
    Assert.Equal("RefreshToken", exception.PropertyName);
    Assert.NotNull(exception.InnerException);
  }

  [Fact(DisplayName = "Handle: it should throw SessionNotFoundException when the session could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_SessionNotFoundException()
  {
    RefreshToken refreshToken = new(Guid.NewGuid(), Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshToken.SecretLength)));
    RenewSessionPayload payload = new(refreshToken.Encode());
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(refreshToken.SessionId, exception.SessionId);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "Handle: it should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    RenewSessionPayload payload = new(string.Empty, [new CustomAttribute("123_Test", "   ")]);
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "RefreshToken");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "CustomAttributes[0].Value");
  }
}
