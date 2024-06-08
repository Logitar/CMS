using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Web.Extensions;

namespace Logitar.Cms.Web;

public class HttpActivityContextResolver : IActivityContextResolver
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  protected HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public HttpActivityContextResolver(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public virtual Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken)
  {
    ActivityContext context = new(_cacheService.GetConfiguration(), Context.GetApiKey(), Context.GetSession(), Context.GetUser());
    return Task.FromResult(context);
  }
}
