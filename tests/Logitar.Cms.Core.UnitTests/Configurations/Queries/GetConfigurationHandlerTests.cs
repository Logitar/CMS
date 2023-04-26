using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Mapping;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Configurations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class GetConfigurationHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IConfigurationRepository> _configurationRepository = new();
  private readonly Mock<IMappingService> _mappingService = new();

  private readonly GetConfigurationHandler _handler;

  public GetConfigurationHandlerTests()
  {
    _handler = new(_configurationRepository.Object, _mappingService.Object);
  }

  [Fact]
  public async Task When_configuration_has_been_loaded_Then_output_is_returned()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken))
      .ReturnsAsync(configuration);

    Configuration output = new();
    _mappingService.Setup(x => x.Map<Configuration?>(configuration))
      .Returns(output);

    GetConfiguration query = new();
    Configuration? result = await _handler.Handle(query, _cancellationToken);

    Assert.Same(output, result);
  }

  [Fact]
  public async Task When_configuration_is_not_loaded_Then_it_returns_null()
  {
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken))
      .ReturnsAsync(value: null);

    GetConfiguration query = new();
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }
}
