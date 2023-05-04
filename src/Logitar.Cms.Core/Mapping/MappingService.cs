using AutoMapper;
using Logitar.Cms.Core.Caching;

namespace Logitar.Cms.Core.Mapping;

internal class MappingService : IMappingService
{
  private readonly ICacheService _cacheService;
  private readonly IMapper _mapper;

  public MappingService(ICacheService cacheService, IMapper mapper)
  {
    _cacheService = cacheService;
    _mapper = mapper;
  }

  public T Map<T>(object? value)
  {
    return _mapper.Map<T>(value, options => options.Items[MappingExtensions.CacheKey] = _cacheService);
  }
}
