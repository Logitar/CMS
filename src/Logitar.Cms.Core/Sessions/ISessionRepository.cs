using Logitar.Cms.Core.Users;

namespace Logitar.Cms.Core.Sessions;

public interface ISessionRepository
{
  Task<IEnumerable<SessionAggregate>> LoadActiveAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken = default);
}
