using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Models.Account;

namespace Logitar.Cms.Web.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken = default);
  Task<Session> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
