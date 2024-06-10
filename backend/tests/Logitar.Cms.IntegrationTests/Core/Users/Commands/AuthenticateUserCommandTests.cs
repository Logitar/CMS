using FluentValidation.Results;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class AuthenticateUserCommandTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public AuthenticateUserCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task It_should_authenticate_the_user()
  {
    AuthenticateUserPayload payload = new(UsernameString, PasswordString);
    AuthenticateUserCommand command = new(payload);
    User user = await Pipeline.ExecuteAsync(command);
    Assert.Equal(UsernameString, user.UniqueName);

    UserEntity? entity = await IdentityContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(user.Id).Value);
    Assert.NotNull(entity);
    Assert.NotNull(entity.AuthenticatedOn);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    AuthenticateUserPayload payload = new(UsernameString, PasswordString[1..]);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple users were found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_users_were_found()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Person.Email));
    await _userRepository.SaveAsync(user);

    AuthenticateUserPayload payload = new(user.UniqueName.Value, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UserHasNoPasswordException when the user has no password.")]
  public async Task It_should_throw_UserHasNoPasswordException_when_the_user_has_no_password()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);
    Assert.False(user.HasPassword);

    AuthenticateUserPayload payload = new(user.UniqueName.Value, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserHasNoPasswordException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserIsDisabledException when the user is disabled.")]
  public async Task It_should_throw_UserIsDisabledException_when_the_user_is_disabled()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    user.Disable();
    await _userRepository.SaveAsync(user);

    AuthenticateUserPayload payload = new(UsernameString, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserIsDisabledException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when none was found.")]
  public async Task It_should_throw_UserNotFoundException_when_none_was_found()
  {
    AuthenticateUserPayload payload = new(UsernameString[1..], PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.Username, exception.UniqueName);
    Assert.Equal(nameof(payload.Username), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateUserPayload payload = new(UsernameString, password: string.Empty);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal(nameof(payload.Password), failure.PropertyName);
  }
}
