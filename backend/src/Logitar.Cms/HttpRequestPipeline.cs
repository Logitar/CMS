using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Web.Extensions;
using MediatR;

namespace Logitar.Cms;

internal class HttpRequestPipeline : IRequestPipeline
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ISender _sender;

  public HttpRequestPipeline(ICacheService cacheService, IHttpContextAccessor httpContextAccessor, ISender sender)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
    _sender = sender;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is IActivity activity && _httpContextAccessor.HttpContext != null)
    {
      HttpContext httpContext = _httpContextAccessor.HttpContext;

      Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
      ActivityContext context = new(configuration, httpContext.GetUser(), httpContext.GetSession());
      activity.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
