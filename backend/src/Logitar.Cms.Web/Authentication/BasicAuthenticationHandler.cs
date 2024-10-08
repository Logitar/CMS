using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Web.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Cms.Web.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly IRequestPipeline _requestPipeline;

  public BasicAuthenticationHandler(IRequestPipeline requestPipeline, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
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
        else if (values[0] == Schemes.Basic)
        {
          byte[] bytes = Convert.FromBase64String(values[1]);
          string credentials = Encoding.UTF8.GetString(bytes);
          int index = credentials.IndexOf(':');
          if (index <= 0)
          {
            return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
          }

          try
          {
            AuthenticateUserPayload payload = new(username: credentials[..index], password: credentials[(index + 1)..]);
            AuthenticateUserCommand command = new(payload);
            UserModel user = await _requestPipeline.ExecuteAsync(command);

            Context.SetUser(user);

            ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
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
