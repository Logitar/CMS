using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Mapping;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Queries;

internal class GetConfigurationHandler : IRequestHandler<GetConfiguration, Configuration?>
{
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMappingService _mappingService;

  public GetConfigurationHandler(IConfigurationRepository configurationRepository, IMappingService mappingService)
  {
    _configurationRepository = configurationRepository;
    _mappingService = mappingService;
  }

  public async Task<Configuration?> Handle(GetConfiguration request, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);

    return _mappingService.Map<Configuration?>(configuration);
  }
}
