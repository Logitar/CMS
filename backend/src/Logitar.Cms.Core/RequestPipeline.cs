using Logitar.Cms.Core.Logging;
using MediatR;

namespace Logitar.Cms.Core;

internal class RequestPipeline : IRequestPipeline
{
  private readonly IActivityContextResolver _contextResolver;
  private readonly ILoggingService _loggingService;
  private readonly ISender _sender;

  public RequestPipeline(IActivityContextResolver contextResolver, ILoggingService loggingService, ISender sender)
  {
    _contextResolver = contextResolver;
    _loggingService = loggingService;
    _sender = sender;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is IActivity activity)
    {
      ActivityContext context = await _contextResolver.ResolveAsync(cancellationToken);
      activity.Contextualize(context);

      _loggingService.SetActivity(activity);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
