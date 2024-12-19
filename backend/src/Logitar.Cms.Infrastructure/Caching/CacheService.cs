using Logitar.Cms.Core.Models;
using Logitar.Cms.Infrastructure.Settings;
using Logitar.EventSourcing;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Cms.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _cache;
  private readonly CachingSettings _settings;

  public CacheService(IMemoryCache cache, CachingSettings settings)
  {
    _cache = cache;
    _settings = settings;
  }

  public ActorModel? GetActor(ActorId id) => TryGetValue<ActorModel>(GetActorKey(id));
  public void SetActor(ActorModel actor)
  {
    SetValue(GetActorKey(new ActorId(actor.Id)), actor, _settings.ActorLifetime);
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";

  private T? TryGetValue<T>(object key) => _cache.TryGetValue(key, out object? value) ? (T?)value : default;
  void SetValue<T>(object key, T value, TimeSpan duration)
  {
    _cache.Set(key, value, duration);
  }
}
