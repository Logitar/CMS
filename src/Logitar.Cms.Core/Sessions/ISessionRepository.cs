namespace Logitar.Cms.Core.Sessions;

public interface ISessionRepository
{
  Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
