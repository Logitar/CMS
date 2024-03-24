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
    if (request is IActivity activity && _httpContextAccessor.HttpContext != null)
    {
      ActivityContext context = new(User: null); // TODO(fpion): Authentication
      activity.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
