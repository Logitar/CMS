using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using MediatR;

namespace Logitar.Cms;

internal class TestRequestPipeline : IRequestPipeline
{
  private readonly ICacheService _cacheService;
  private readonly TestContext _context;
  private readonly ISender _sender;

  public TestRequestPipeline(ICacheService cacheService, TestContext context, ISender sender)
  {
    _cacheService = cacheService;
    _context = context;
    _sender = sender;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is IActivity activity)
    {
      Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
      ActivityContext context = new(configuration, _context.User, Session: null);
      activity.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
