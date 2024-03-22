using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Core;
using MediatR;

namespace Logitar.Cms;

internal class HttpRequestPipeline : IRequestPipeline
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ISender _sender;

  public HttpRequestPipeline(IHttpContextAccessor httpContextAccessor, ISender sender)
  {
    _httpContextAccessor = httpContextAccessor;
    _sender = sender;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is Request applicationRequest && _httpContextAccessor.HttpContext != null)
    {
      RequestContext context = new(Actor.System); // TODO(fpion): Authentication
      applicationRequest.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
