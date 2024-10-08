using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Core;
using Logitar.Cms.Web.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Cms.Web.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
  private readonly IRequestPipeline _requestPipeline;

  public ApiKeyAuthenticationHandler(IRequestPipeline requestPipeline, IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _requestPipeline = requestPipeline;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.ApiKey, out StringValues values))
    {
      string? value = values.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        try
        {
          AuthenticateApiKeyPayload payload = new(value);
          AuthenticateApiKeyCommand command = new(payload);
          ApiKeyModel apiKey = await _requestPipeline.ExecuteAsync(command);

          Context.SetApiKey(apiKey);

          ClaimsPrincipal principal = new(apiKey.CreateClaimsIdentity(Scheme.Name));
          AuthenticationTicket ticket = new(principal, Scheme.Name);

          return AuthenticateResult.Success(ticket);
        }
        catch (Exception exception)
        {
          return AuthenticateResult.Fail(exception);
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
