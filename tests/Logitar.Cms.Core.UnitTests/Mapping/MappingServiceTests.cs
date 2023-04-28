using AutoMapper;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Mapping;

[Trait(Traits.Category, Categories.Unit)]
public class MappingServiceTests
{
  private readonly Mock<ICacheService> _cacheService = new();
  private readonly IMapper _mapper;

  private readonly MappingService _service;

  public MappingServiceTests()
  {
    _mapper = new Mapper(new MapperConfiguration(options => options.AddProfile<ConfigurationProfile>()));

    _service = new(_cacheService.Object, _mapper);
  }

  [Fact]
  public void When_source_is_not_null_Then_it_is_correctly_mapped()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());

    Actor actor = new();
    _cacheService.Setup(x => x.GetActor(configuration.CreatedById))
      .Returns(actor);

    Configuration output = _service.Map<Configuration>(configuration);

    Assert.Same(actor, output.CreatedBy);
    Assert.Same(actor, output.UpdatedBy);
  }

  [Fact]
  public void When_source_is_null_Then_destination_is_null()
  {
    ConfigurationAggregate? configuration = null;

    Assert.Null(_service.Map<Configuration?>(configuration));
  }
}
