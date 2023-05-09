using Logitar.Cms.Contracts.Sessions;

namespace Logitar.Cms.Core.Sessions;

public interface ISessionQuerier
{
  Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<IEnumerable<Session>> GetAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken = default);
  Task<Session?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
