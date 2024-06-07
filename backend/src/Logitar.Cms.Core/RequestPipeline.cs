using MediatR;

namespace Logitar.Cms.Core;

public class RequestPipeline : IRequestPipeline
{
  private readonly IActivityContextResolver _contextResolver;
  private readonly ISender _sender;

  public RequestPipeline(IActivityContextResolver contextResolver, ISender sender)
  {
    _contextResolver = contextResolver;
    _sender = sender;
  }

  public virtual async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is IActivity activity)
    {
      ActivityContext context = await _contextResolver.ResolveAsync(cancellationToken);
      activity.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
