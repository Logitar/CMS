using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
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
  public void SetActor(UserAggregate user)
  {
    Actor actor = new()
    {
      Id = user.Id.Value,
      Type = ActorType.User,
      IsDeleted = user.IsDeleted,
      DisplayName = user.FullName ?? user.Username,
      EmailAddress = user.Email?.Address,
      PictureUrl = user.Picture?.ToString()
    };
    SetItem(GetActorKey(user.Id), actor);
  }
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
