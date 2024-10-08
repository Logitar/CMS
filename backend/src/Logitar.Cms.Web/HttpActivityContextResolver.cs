using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;

namespace Logitar.Cms.Web;

public class HttpActivityContextResolver : IActivityContextResolver
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  private HttpContext HttpContext => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public HttpActivityContextResolver(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken)
  {
    ActivityContext context = new(_cacheService.GetConfiguration(), HttpContext.GetApiKey(), HttpContext.GetSession(), HttpContext.GetUser());
    return Task.FromResult(context);
  }
}
