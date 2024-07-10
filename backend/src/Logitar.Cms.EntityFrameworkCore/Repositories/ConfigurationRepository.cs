using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class ConfigurationRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IConfigurationRepository
{
  public ConfigurationRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
  {
    AggregateId id = new ConfigurationId().AggregateId;
    return await base.LoadAsync<ConfigurationAggregate>(id, cancellationToken);
  }

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);
  }
}
