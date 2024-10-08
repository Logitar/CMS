using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;

namespace Logitar.Cms.Core.Sessions;

public interface ISessionQuerier
{
  Task<SessionModel> ReadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
