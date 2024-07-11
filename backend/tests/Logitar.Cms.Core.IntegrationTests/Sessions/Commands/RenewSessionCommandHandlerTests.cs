using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class RenewSessionCommandHandlerTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public RenewSessionCommandHandlerTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should renew an existing persistent session.")]
  public async Task It_should_renew_an_existing_persistent_session()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    SessionAggregate session = new(user, secret);
    await _sessionRepository.SaveAsync(session);

    RenewSessionPayload payload = new(RefreshToken.Encode(session, secretString),
    [
      new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
      new CustomAttribute("IpAddress", Faker.Internet.Ip())
    ]);
    RenewSessionCommand command = new(payload);
    Session result = await Pipeline.ExecuteAsync(command);

    Assert.Equal(session.Id.ToGuid(), result.Id);
    Assert.Equal(session.Version + 2, result.Version);
    Assert.Equal(session.CreatedBy.ToGuid(), result.CreatedBy.Id);
    Assert.Equal(session.CreatedOn.AsUniversalTime(), result.CreatedOn);
    Assert.Equal(user.Id.ToGuid(), result.UpdatedBy.Id);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.True(result.IsPersistent);
    Assert.NotNull(result.RefreshToken);
    Assert.True(result.IsActive);
    Assert.Equal(payload.CustomAttributes, result.CustomAttributes);
    Assert.Equal(user.Id.ToGuid(), result.User.Id);
  }
}
