using Bogus;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateUserCommandHandlerTests
{
  //private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IAddressHelper> _addressHelper = new();
  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IMediator> _mediator = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<IUserManager> _userManager = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();
  private readonly Mock<IUserSettingsResolver> _userSettingsResolver = new();

  private readonly UpdateUserCommandHandler _handler;

  public UpdateUserCommandHandlerTests()
  {
    _handler = new(_addressHelper.Object, _applicationContext.Object, _mediator.Object, _passwordManager.Object, _userManager.Object, _userQuerier.Object, _userRepository.Object, _userSettingsResolver.Object);
  }

  // TODO(fpion): implement
}
