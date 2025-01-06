using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Cms.Web.Authentication;

public class BearerAuthenticationOptions : AuthenticationSchemeOptions;

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
      foreach (string? value in authorization)
      {
        if (!string.IsNullOrWhiteSpace(value))
        {
          string[] values = value.Trim().Split();
          if (values.Length != 2)
          {
            return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
          }
          else if (values[0] == Schemes.Bearer)
          {
            try
            {
              string accessToken = values[1];
              SessionModel session = await _openAuthenticationService.GetSessionAsync(accessToken);
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
    }

    return AuthenticateResult.NoResult();
  }
}
