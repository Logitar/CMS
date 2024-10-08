using Logitar.Cms.Core.Logging;

namespace Logitar.Cms;

internal class LogRepository : ILogRepository
{
  public Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
