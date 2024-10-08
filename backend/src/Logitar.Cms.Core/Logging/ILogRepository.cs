namespace Logitar.Cms.Core.Logging;

public interface ILogRepository
{
  Task SaveAsync(Log log, CancellationToken cancellationToken = default);
}
