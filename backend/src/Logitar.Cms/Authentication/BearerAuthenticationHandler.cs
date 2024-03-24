using Logitar.Cms.Constants;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Queries;
using Logitar.Cms.Core.Users.Queries;
using Logitar.Cms.Extensions;
using Logitar.Cms.Web.Extensions;
using Logitar.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Cms.Authentication;

internal class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly IBearerService _bearerService;
  private readonly IRequestPipeline _requestPipeline;

  public BearerAuthenticationHandler(IBearerService bearerService, IRequestPipeline requestPipeline, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _bearerService = bearerService;
    _requestPipeline = requestPipeline;
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
            ClaimsPrincipal principal = await _bearerService.ValidateAsync(values[1]);

            Claim? sessionId = FindClaim(principal, Rfc7519ClaimNames.SessionId);
            if (sessionId != null)
            {
              ReadSessionQuery query = new(Guid.Parse(sessionId.Value));
              Session? session = await _requestPipeline.ExecuteAsync(query);
              if (session == null)
              {
                return AuthenticateResult.Fail($"The session 'Id={sessionId}' could not be found.");
              }
              else if (!session.IsActive)
              {
                return AuthenticateResult.Fail($"The session 'Id={session.Id}' has ended.");
              }
              else if (session.User.IsDisabled)
              {
                return AuthenticateResult.Fail($"The User is disabled for session 'Id={session.Id}'.");
              }

              Context.SetSession(session);
              Context.SetUser(session.User);

              principal = new(session.CreateClaimsIdentity(Scheme.Name));
            }

            Claim? userId = FindClaim(principal, Rfc7519ClaimNames.Subject);
            Claim? username = FindClaim(principal, Rfc7519ClaimNames.Username);
            if (userId != null || username != null)
            {
              ReadUserQuery query = new(userId == null ? null : Guid.Parse(userId.Value), username?.Value);
              User? user = await _requestPipeline.ExecuteAsync(query);
              if (user == null)
              {
                return AuthenticateResult.Fail($"The user 'Id={userId?.Value}, Username={username?.Value}' could not be found.");
              }
              else if (user.IsDisabled)
              {
                return AuthenticateResult.Fail($"The User 'Id={user.Id}' is disabled.");
              }

              Context.SetUser(user);

              principal = new(user.CreateClaimsIdentity(Scheme.Name));
            }

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

  private static Claim? FindClaim(ClaimsPrincipal principal, string name)
  {
    IEnumerable<Claim> claims = principal.FindAll(name);
    int count = claims.Count();
    if (count == 0)
    {
      return null;
    }
    else if (count > 1)
    {
      throw new InvalidOperationException($"{count} '{name}' claims were found when one or none was expected.");
    }
    return claims.Single();
  }
}
