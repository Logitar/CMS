using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;

namespace Logitar.Cms.Core.Sessions;

public interface ISessionQuerier
{
  Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
