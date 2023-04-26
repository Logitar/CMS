namespace Logitar.Cms.Core.Configurations;

public interface IConfigurationRepository
{
  Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken = default);
}
