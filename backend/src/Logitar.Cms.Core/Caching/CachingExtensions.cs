using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Caching;

public static class CachingExtensions
{
  public static Configuration GetConfiguration(this ICacheService cache)
  {
    return cache.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  }
}
