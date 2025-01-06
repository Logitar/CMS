using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Web.Models.Account;

namespace Logitar.Cms.Web.Authentication;

public interface IOpenAuthenticationService
{
  Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken = default);
  Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken = default);
}
