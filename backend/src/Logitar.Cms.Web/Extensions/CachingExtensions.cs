using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;

namespace Logitar.Cms.Web.Extensions;

public static class CachingExtensions
{
  public static Configuration GetConfiguration(this ICacheService cache)
  {
    return cache.Configuration ?? throw new ArgumentException("The configuration was not found in the cache.", nameof(cache));
  }
}
