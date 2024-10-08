using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Cms.Web.Authentication;

public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly IOpenAuthenticationService _openAuthenticationService;

  public BearerAuthenticationHandler(IOpenAuthenticationService openAuthenticationService, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _openAuthenticationService = openAuthenticationService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Schemes.Bearer)
        {
          try
          {
            SessionModel session = await _openAuthenticationService.ValidateTokenAsync(values[1]);
            if (!session.IsActive)
            {
              return AuthenticateResult.Fail($"The session 'Id={session.Id}' has ended.");
            }
            else if (session.User.IsDisabled)
            {
              return AuthenticateResult.Fail($"The User is disabled for session 'Id={session.Id}'.");
            }

            Context.SetSession(session);
            Context.SetUser(session.User);

            ClaimsPrincipal principal = new(session.CreateClaimsIdentity(Scheme.Name));
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
