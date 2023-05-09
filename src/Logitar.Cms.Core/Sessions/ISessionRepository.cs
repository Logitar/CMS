namespace Logitar.Cms.Core.Sessions;

public interface ISessionRepository
{
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
