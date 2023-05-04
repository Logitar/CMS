using AutoMapper;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Core.Caching;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Mapping;

internal static class MappingExtensions
{
  public const string CacheKey = "Cache";

  public static Actor GetActor(this ResolutionContext context, AggregateId id)
  {
    if (!context.Items.TryGetValue(CacheKey, out object? value))
    {
      throw new ArgumentException($"The '{CacheKey}' item has not been set.", nameof(context));
    }
    else if (value is not ICacheService cacheService)
    {
      throw new ArgumentException($"The '{CacheKey}' item should implement the '{nameof(ICacheService)}' interface.", nameof(context));
    }
    else
    {
      return cacheService.GetActor(id) ?? throw new InvalidOperationException($"The actor 'Id={id}' could not be found.");
    }
  }
}
