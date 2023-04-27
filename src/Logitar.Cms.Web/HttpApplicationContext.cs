using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Web.Extensions;
using Logitar.EventSourcing;

namespace Logitar.Cms.Web;

internal class HttpApplicationContext : IApplicationContext
{
  private static readonly AggregateId _systemId = new(new Actor().Id);

  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public AggregateId ActorId
  {
    get
    {
      if (_httpContextAccessor.HttpContext != null)
      {
        User? user = _httpContextAccessor.HttpContext.GetUser();
        if (user != null)
        {
          return new AggregateId(user.Id);
        }
      }
      return _systemId;
    }
  }

  public ConfigurationAggregate Configuration => _cacheService.Configuration
    ?? throw new InvalidOperationException("The configuration could not be found in the cache.");
}
