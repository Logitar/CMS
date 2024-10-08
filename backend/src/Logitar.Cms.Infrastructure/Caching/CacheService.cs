using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Infrastructure.Settings;
using Logitar.EventSourcing;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Cms.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;
  private readonly CachingSettings _settings;

  public CacheService(IMemoryCache memoryCache, CachingSettings settings)
  {
    _memoryCache = memoryCache;
    _settings = settings;
  }

  public ConfigurationModel? Configuration
  {
    get => GetItem<ConfigurationModel>(ConfigurationKey);
    set => SetItem(ConfigurationKey, value);
  }
  private const string ConfigurationKey = nameof(Configuration);

  public Actor? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return GetItem<Actor>(key);
  }
  public void SetActor(Actor actor)
  {
    string key = GetActorKey(new ActorId(actor.Id));
    SetItem(key, actor, _settings.ActorLifetime);
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetItem<T>(object key, T value, TimeSpan? lifetime = null)
  {
    if (lifetime.HasValue)
    {
      _memoryCache.Set(key, value, lifetime.Value);
    }
    else
    {
      _memoryCache.Set(key, value);
    }
  }
}
