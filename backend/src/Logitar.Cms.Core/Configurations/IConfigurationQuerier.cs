using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Configurations;

public interface IConfigurationQuerier
{
  Task<ConfigurationModel> ReadAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
