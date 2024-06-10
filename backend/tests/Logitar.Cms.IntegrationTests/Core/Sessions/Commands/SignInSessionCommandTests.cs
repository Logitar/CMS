using FluentValidation.Results;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignInSessionCommandTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public SignInSessionCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should sign-in the specified user.")]
  public async Task It_should_sign_in_the_specified_user()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SignInSessionPayload payload = new(UsernameString, PasswordString)
    {
      IpAddress = Faker.Internet.Ip(),
      AdditionalInformation = $@"  {{""User-Agent"":""{Faker.Internet.UserAgent()}""}}  "
    };
    SignInSessionCommand command = new(payload);
    Session session = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, session.Id);
    Assert.Equal(2, session.Version);
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
    Assert.Equal(new SessionId(session.Id), refreshToken.Id);
    Assert.Equal(32, Convert.FromBase64String(refreshToken.Secret).Length);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many users were found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_users_were_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    user.SetEmail(new EmailUnit(Faker.Person.Email, isVerified: true), ActorId);
    UserAggregate other = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Person.Email), tenantId: null, ActorId);
    await _userRepository.SaveAsync([user, other]);

    SignInSessionPayload payload = new(Faker.Person.Email, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is not correct.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_not_correct()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SignInSessionPayload payload = new(UsernameString, PasswordString[1..]);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw UserHasNoPasswordException when the user has no password.")]
  public async Task It_should_throw_UserHasNoPasswordException_when_the_user_has_no_password()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()), tenantId: null, ActorId);
    await _userRepository.SaveAsync(user);

    SignInSessionPayload payload = new(user.UniqueName.Value, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserHasNoPasswordException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserIsDisabledException when the user is disabled.")]
  public async Task It_should_throw_UserIsDisabledException_when_the_user_is_disabled()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    user.Disable(ActorId);
    await _userRepository.SaveAsync(user);

    SignInSessionPayload payload = new(UsernameString, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserIsDisabledException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user could not be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_could_not_be_found()
  {
    SignInSessionPayload payload = new(UsernameString[1..], PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.Username, exception.UniqueName);
    Assert.Equal(nameof(payload.Username), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SignInSessionPayload payload = new(UsernameString, password: string.Empty);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.Password), error.PropertyName);
  }
}
