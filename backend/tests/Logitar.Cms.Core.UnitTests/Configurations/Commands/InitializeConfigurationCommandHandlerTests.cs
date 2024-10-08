using Bogus;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Languages.Commands;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Users;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Configurations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class InitializeConfigurationCommandHandlerTests
{
  private const string PasswordString = "Test123!";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICacheService> _cacheService = new();
  private readonly Mock<IConfigurationQuerier> _configurationQuerier = new();
  private readonly Mock<IConfigurationRepository> _configurationRepository = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly InitializeConfigurationCommandHandler _handler;

  public InitializeConfigurationCommandHandlerTests()
  {
    _handler = new(
      _cacheService.Object,
      _configurationQuerier.Object,
      _configurationRepository.Object,
      _passwordManager.Object,
      _sender.Object,
      _userManager.Object);
  }

  [Fact(DisplayName = "It should initialize the configuration.")]
  public async Task It_should_initialize_the_configuration()
  {
    PasswordMock password = new(PasswordString);
    _passwordManager.Setup(x => x.ValidateAndCreate(PasswordString)).Returns(password);

    InitializeConfigurationCommand command = new(_faker.Locale, _faker.Person.UserName, PasswordString);

    await _handler.Handle(command, _cancellationToken);

    _userManager.Verify(x => x.SaveAsync(
      It.Is<UserAggregate>(y => y.TenantId == null && y.UniqueName.Value == command.Username && y.HasPassword && y.CreatedBy.Value == y.Id.Value),
      It.IsAny<IUserSettings>(),
      It.IsAny<ActorId>(),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.IsDefault && y.Language.Locale.Code == command.DefaultLocale),
      _cancellationToken), Times.Once);

    _configurationRepository.Verify(x => x.LoadAsync(_cancellationToken), Times.Once);
    _configurationRepository.Verify(x => x.SaveAsync(It.IsAny<Configuration>(), _cancellationToken), Times.Once);

    _configurationQuerier.Verify(x => x.ReadAsync(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should only cache the configuration when it is already initialized.")]
  public async Task It_should_only_cache_the_configuration_when_it_is_already_initialized()
  {
    Configuration configuration = Configuration.Initialize(ActorId.NewId());
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken)).ReturnsAsync(configuration);

    ConfigurationModel model = new();
    _configurationQuerier.Setup(x => x.ReadAsync(configuration, _cancellationToken)).ReturnsAsync(model);

    InitializeConfigurationCommand command = new(_faker.Locale, _faker.Person.UserName, PasswordString);

    await _handler.Handle(command, _cancellationToken);

    _cacheService.VerifySet(x => x.Configuration = model);

    _userManager.Verify(x => x.SaveAsync(It.IsAny<UserAggregate>(), It.IsAny<IUserSettings>(), It.IsAny<ActorId>(), It.IsAny<CancellationToken>()), Times.Never);
    _sender.Verify(x => x.Send(It.IsAny<SaveLanguageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    _configurationRepository.Verify(x => x.SaveAsync(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
