using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Configurations;

public interface IConfigurationQuerier
{
  Task<Configuration> ReadAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken = default);
}
