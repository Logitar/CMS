using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Models.Account;

namespace Logitar.Cms.Authentication;

public interface IBearerService
{
  Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken);
}
