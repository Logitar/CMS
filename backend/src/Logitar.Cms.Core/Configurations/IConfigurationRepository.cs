namespace Logitar.Cms.Core.Configurations;

public interface IConfigurationRepository
{
  Task<Configuration?> LoadAsync(CancellationToken cancellationToken = default);

  Task SaveAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
