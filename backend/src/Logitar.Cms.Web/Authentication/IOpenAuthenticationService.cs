using Logitar.Cms.Contracts.Account;
using Logitar.Cms.Contracts.Sessions;

namespace Logitar.Cms.Web.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken = default);
  Task<SessionModel> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
