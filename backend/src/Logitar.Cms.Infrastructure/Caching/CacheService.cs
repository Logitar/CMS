using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Cms.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private const string ConfigurationKey = nameof(Configuration);

  private readonly IMemoryCache _cache;

  public CacheService(IMemoryCache cache)
  {
    _cache = cache;
  }

  public Configuration? Configuration
  {
    get => GetItem<Configuration>(ConfigurationKey);
    set => SetItem(ConfigurationKey, value);
  }

  private T? GetItem<T>(object key) => _cache.TryGetValue(key, out T? value) ? value : default;
  private void SetItem<T>(object key, T value) => _cache.Set(key, value);
}
