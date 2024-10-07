using Logitar.Cms.Core.Caching;

namespace Logitar.Cms.Core;

internal class TestActivityContextResolver : IActivityContextResolver
{
  private readonly ICacheService _cacheService;
  private readonly TestContext _context;

  public TestActivityContextResolver(ICacheService cacheService, TestContext context)
  {
    _cacheService = cacheService;
    _context = context;
  }

  public Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken)
  {
    ActivityContext context = new(_cacheService.GetConfiguration(), ApiKey: null, Session: null, _context.User);
    return Task.FromResult(context);
  }
}
