using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;

internal class ConfigurationRepository : EventStore, IConfigurationRepository
{
  private readonly ICacheService _cacheService;

  public ConfigurationRepository(ICacheService cacheService,
    EventContext context,
    IEventBus eventBus) : base(context, eventBus)
  {
    _cacheService = cacheService;
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<ConfigurationAggregate>(ConfigurationAggregate.GlobalId, cancellationToken);
  }

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);

    _cacheService.Configuration = configuration;
  }
}
