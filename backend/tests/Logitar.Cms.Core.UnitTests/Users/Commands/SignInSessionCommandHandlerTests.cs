using Bogus;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users.Queries;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class AuthenticateUserCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly AuthenticateUserCommandHandler _handler;

  public AuthenticateUserCommandHandlerTests()
  {
    _handler = new(_sender.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), _faker.Person.UserName));
    user.SetPassword(new PasswordMock("Test123!"));
    _sender.Setup(x => x.Send(It.Is<FindUserQuery>(q => q.UniqueName == user.UniqueName.Value && q.PropertyName == "UniqueName"),
      _cancellationToken)).ReturnsAsync(user);

    AuthenticateUserPayload payload = new(user.UniqueName.Value, "AAaa!!11");
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateUserPayload payload = new("admin", string.Empty);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Password", error.PropertyName);
  }
}
