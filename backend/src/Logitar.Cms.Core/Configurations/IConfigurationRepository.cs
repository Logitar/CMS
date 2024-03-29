﻿namespace Logitar.Cms.Core.Configurations;

public interface IConfigurationRepository
{
  Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken = default);

  Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken = default);
}
