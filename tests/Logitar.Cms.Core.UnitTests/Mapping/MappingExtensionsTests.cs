using AutoMapper;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Moq;

namespace Logitar.Cms.Core.Mapping;

[Trait(Traits.Category, Categories.Unit)]
public class MappingExtensionsTests
{
  private readonly Mock<ICacheService> _cacheService = new();

  private readonly IMapper _mapper;

  public MappingExtensionsTests()
  {
    _mapper = new Mapper(new MapperConfiguration(options => options.AddProfile<ConfigurationProfile>()));
  }

  [Fact]
  public void When_actor_has_been_found_Then_aggregate_is_mapped()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());

    Actor actor = new();
    _cacheService.Setup(x => x.GetActor(configuration.CreatedById))
      .Returns(actor);

    Configuration output = _mapper.Map<Configuration>(configuration,
      options => options.Items[MappingExtensions.CacheKey] = _cacheService.Object);

    Assert.Same(actor, output.CreatedBy);
    Assert.Same(actor, output.UpdatedBy);
  }

  [Fact]
  public void When_actor_has_not_been_found_Then_InvalidOperationException_is_thrown()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());

    var exception = Assert.Throws<AutoMapperMappingException>(() => _mapper.Map<Configuration>(configuration,
      options => options.Items[MappingExtensions.CacheKey] = _cacheService.Object));

    Assert.NotNull(exception.InnerException);
    Assert.IsType<InvalidOperationException>(exception.InnerException);
    Assert.Equal($"The actor 'Id={configuration.CreatedById}' could not be found.", exception.InnerException.Message);
  }

  [Fact]
  public void When_cache_has_not_been_set_Then_ArgumentException_is_thrown()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());

    var exception = Assert.Throws<AutoMapperMappingException>(() => _mapper.Map<Configuration>(configuration,
      options => options.Items[$"_{MappingExtensions.CacheKey}"] = _cacheService.Object));

    Assert.NotNull(exception.InnerException);
    Assert.IsType<ArgumentException>(exception.InnerException);
    Assert.Equal("context", ((ArgumentException)exception.InnerException).ParamName);
  }

  [Fact]
  public void When_cache_is_not_a_cache_service_Then_ArgumentException_is_thrown()
  {
    ConfigurationAggregate configuration = new(AggregateId.NewId());

    var exception = Assert.Throws<AutoMapperMappingException>(() => _mapper.Map<Configuration>(configuration,
      options => options.Items[MappingExtensions.CacheKey] = _cacheService));

    Assert.NotNull(exception.InnerException);
    Assert.IsType<ArgumentException>(exception.InnerException);
    Assert.Equal("context", ((ArgumentException)exception.InnerException).ParamName);
  }
}
