using Logitar.Cms.Core.Sessions.Models;
using Logitar.Identity.Core.Sessions;

namespace Logitar.Cms.Core.Sessions;

public interface ISessionQuerier
{
  Task<SessionModel> ReadAsync(Session session, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
