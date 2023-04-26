using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Caching.Commands;

internal class InitializeCachingHandler : INotificationHandler<InitializeCaching>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IUserRepository _userRepository;

  public InitializeCachingHandler(ICacheService cacheService,
    IConfigurationRepository configurationRepository,
    IUserRepository userRepository)
  {
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
    _userRepository = userRepository;
  }

  public async Task Handle(InitializeCaching notification, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      _cacheService.Configuration = configuration;

      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(includeDeleted: true, cancellationToken);
      foreach (UserAggregate user in users)
      {
        _cacheService.SetActor(user);
      }
    }
  }
}
