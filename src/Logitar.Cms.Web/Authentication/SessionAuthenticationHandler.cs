using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Claims;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Logitar.Cms.Web.Authentication;

internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
{
  private readonly ICacheService _cacheService;
  private readonly ISessionQuerier _sessionQuerier;

  public SessionAuthenticationHandler(IOptionsMonitor<SessionAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    ICacheService cacheService,
    ISessionQuerier sessionQuerier) : base(options, logger, encoder, clock)
  {
    _cacheService = cacheService;
    _sessionQuerier = sessionQuerier;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    Guid? sessionId = Context.GetSessionId();
    if (sessionId.HasValue)
    {
      Session? session = /*_cacheService.GetSession(sessionId.Value) ??*/ await _sessionQuerier.GetAsync(sessionId.Value); // TODO(fpion): caching
      AuthenticateResult? failure = null;
      if (session == null)
      {
        failure = AuthenticateResult.Fail($"The session 'Id={sessionId}' could not be found.");
      }
      else if (!session.IsActive)
      {
        failure = AuthenticateResult.Fail($"The session 'Id={session.Id}' has ended.");
      }
      else if (session.User.IsDisabled)
      {
        failure = AuthenticateResult.Fail($"The user 'Id={session.User.Id}' is disabled for session 'Id={session.Id}'.");
      }

      if (failure != null)
      {
        Context.SignOut();

        return failure;
      }

      //_cacheService.SetSession(session); // TODO(fpion): caching

      Context.SetSession(session);
      Context.SetUser(session!.User);

      ClaimsPrincipal principal = new(session.User.GetClaimsIdentity(Schemes.Session));
      AuthenticationTicket ticket = new(principal, Schemes.Session);

      return AuthenticateResult.Success(ticket);
    }

    return AuthenticateResult.NoResult();
  }
}
