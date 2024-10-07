using Logitar.Cms.Contracts.Account;
using Logitar.Cms.Contracts.Sessions;

namespace Logitar.Cms.Web.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken = default);
  Task<Session> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
