using Bogus;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Moq;
using System.Globalization;

namespace Logitar.Cms.Core.Configurations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class InitializeConfigurationHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IConfigurationRepository> _configurationRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IMappingService> _mappingService = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly InitializeConfigurationHandler _handler;

  public InitializeConfigurationHandlerTests()
  {
    _handler = new(_configurationRepository.Object, _languageRepository.Object, _mappingService.Object, _userRepository.Object);
  }

  [Fact]
  public async Task When_configuration_has_not_been_initialized_Then_it_initializes_it_and_return_output()
  {
    Configuration output = new();
    _mappingService.Setup(x => x.Map<Configuration>(It.IsAny<ConfigurationAggregate>()))
      .Returns(output);

    CultureInfo defaultLocale = CultureInfo.GetCultureInfo("fr-CA");
    InitializeConfigurationInput input = new()
    {
      DefaultLocale = defaultLocale.Name,
      User = new InitialUserInput
      {
        Username = _faker.Person.UserName,
        Password = "P@s$W0rD",
        EmailAddress = _faker.Person.Email,
        FirstName = _faker.Person.FirstName,
        LastName = _faker.Person.LastName
      }
    };

    Configuration configuration = await _handler.Handle(new InitializeConfiguration(input), _cancellationToken);
    Assert.Same(output, configuration);

    _configurationRepository.Verify(x => x.SaveAsync(It.Is<ConfigurationAggregate>(c => c.CreatedById.Value != "SYSTEM"
      && c.UpdatedById.Value != "SYSTEM"
      && !string.IsNullOrEmpty(c.Secret)), _cancellationToken), Times.Once);

    _languageRepository.Verify(x => x.SaveAsync(It.Is<LanguageAggregate>(l => l.CreatedById.Value != "SYSTEM"
      && l.UpdatedById.Value != "SYSTEM"
      && l.Locale == defaultLocale
      && l.IsDefault), _cancellationToken), Times.Once);

    _userRepository.Verify(x => x.SaveAsync(It.Is<UserAggregate>(u => u.CreatedById.Value != "SYSTEM"
      && u.UpdatedById.Value != "SYSTEM"
      && u.Username == input.User.Username
      && u.HasPassword
      && u.Email != null && u.Email.Address == input.User.EmailAddress
      && u.FirstName == input.User.FirstName
      && u.LastName == input.User.LastName
      && u.Locale == defaultLocale), _cancellationToken), Times.Once);
  }

  [Fact]
  public async Task When_configuration_is_already_initialized_Then_ConfigurationAlreadyInitializedException_is_thrown()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken))
      .ReturnsAsync(configuration);

    InitializeConfiguration command = new(new InitializeConfigurationInput());
    await Assert.ThrowsAsync<ConfigurationAlreadyInitializedException>(async () => await _handler.Handle(command, _cancellationToken));
  }
}
