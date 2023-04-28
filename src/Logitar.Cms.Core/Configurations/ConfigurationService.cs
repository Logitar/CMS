using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Configurations.Commands;
using Logitar.Cms.Core.Configurations.Queries;

namespace Logitar.Cms.Core.Configurations;

internal class ConfigurationService : IConfigurationService
{
  private readonly IRequestPipeline _pipeline;

  public ConfigurationService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Configuration?> GetAsync(CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetConfiguration(), cancellationToken);
  }

  public async Task<Configuration> InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new InitializeConfiguration(input), cancellationToken);
  }
}
