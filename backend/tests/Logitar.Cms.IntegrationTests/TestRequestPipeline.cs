using Logitar.Cms.Core;
using MediatR;

namespace Logitar.Cms;

internal class TestRequestPipeline : IRequestPipeline
{
  private readonly TestContext _context;
  private readonly ISender _sender;

  public TestRequestPipeline(TestContext context, ISender sender)
  {
    _context = context;
    _sender = sender;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is IActivity activity)
    {
      ActivityContext context = new(_context.User);
      activity.Contextualize(context);
    }

    return await _sender.Send(request, cancellationToken);
  }
}
