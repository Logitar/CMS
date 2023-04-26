using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Cms.Core.Caching;

internal class CacheService : ICacheService
{
  private const string ConfigurationKey = nameof(Configuration);

  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public ConfigurationAggregate? Configuration
  {
    get => GetItem<ConfigurationAggregate>(ConfigurationKey);
    set => SetItem(ConfigurationKey, value);
  }

  public Actor? GetActor(AggregateId id) => GetItem<Actor>(GetActorKey(id));
  private static string GetActorKey(AggregateId id) => $"Actor:{id}";

  private T? GetItem<T>(object key) => _memoryCache.Get<T>(key);
  private void SetItem<T>(object key, T? value)
  {
    if (value == null)
    {
      _memoryCache.Remove(key);
    }
    else
    {
      _memoryCache.Set(key, value);
    }
  }
}
