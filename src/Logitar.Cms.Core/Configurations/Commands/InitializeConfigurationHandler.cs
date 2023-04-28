using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using MediatR;
using System.Globalization;

namespace Logitar.Cms.Core.Configurations.Commands;

internal class InitializeConfigurationHandler : IRequestHandler<InitializeConfiguration, Configuration>
{
  private readonly IConfigurationRepository _configurationRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMappingService _mappingService;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationHandler(IConfigurationRepository configurationRepository,
    ILanguageRepository languageRepository,
    IMappingService mappingService,
    IUserRepository userRepository)
  {
    _configurationRepository = configurationRepository;
    _languageRepository = languageRepository;
    _mappingService = mappingService;
    _userRepository = userRepository;
  }

  public async Task<Configuration> Handle(InitializeConfiguration request, CancellationToken cancellationToken)
  {
    if (await _configurationRepository.LoadAsync(cancellationToken) != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    InitializeConfigurationInput input = request.Input;
    CultureInfo defaultLocale = input.DefaultLocale.GetCultureInfo(nameof(input.DefaultLocale));

    InitialUserInput userInput = input.User;
    AggregateId userId = AggregateId.NewId();

    ConfigurationAggregate configuration = new(actorId: userId);

    UserAggregate user = new(userId, configuration, userInput.Username, userInput.FirstName,
      userInput.LastName, defaultLocale, id: userId);
    user.ChangePassword(configuration, userInput.Password);
    user.SetEmail(new ReadOnlyEmail(userInput.EmailAddress));

    LanguageAggregate language = new(userId, defaultLocale);
    language.SetDefault(userId);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userRepository.SaveAsync(user, cancellationToken);
    await _languageRepository.SaveAsync(language, cancellationToken);

    return _mappingService.Map<Configuration>(configuration);
  }
}
