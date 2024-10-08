using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ConfigurationRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IConfigurationRepository
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;

  public ConfigurationRepository(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IEventBus eventBus,
    EventContext eventContext,
    IEventSerializer eventSerializer
  ) : base(eventBus, eventContext, eventSerializer)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
  }

  public async Task<Configuration?> LoadAsync(CancellationToken cancellationToken)
  {
    ConfigurationId id = new();
    return await base.LoadAsync<Configuration>(id.AggregateId, cancellationToken);
  }

  public async Task SaveAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);

    _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
