using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using MediatR;

namespace Logitar.Cms.Web;

public class HttpRequestPipeline : IRequestPipeline
{
  private readonly ICacheService _cacheService;
  private readonly ISender _sender;

  public HttpRequestPipeline(ICacheService cacheService, ISender sender)
  {
    _cacheService = cacheService;
    _sender = sender;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is IActivity activity)
    {
      if (_cacheService.Configuration == null)
      {
        throw new InvalidOperationException("The configuration was not found in the cache.");
      }

      ActivityContext context = new(_cacheService.Configuration);
      activity.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
