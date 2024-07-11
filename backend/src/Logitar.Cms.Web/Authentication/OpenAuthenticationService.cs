using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Models.Account;
using Logitar.Cms.Web.Settings;
using Logitar.Identity.Domain.Tokens;
using Logitar.Security.Claims;

namespace Logitar.Cms.Web.Authentication;

internal class OpenAuthenticationService : IOpenAuthenticationService
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly OpenAuthenticationSettings _settings;
  private readonly ITokenManager _tokenManager;

  public OpenAuthenticationService(
    ICacheService cacheService,
    IHttpContextAccessor httpContextAccessor,
    ISessionQuerier sessionQuerier,
    OpenAuthenticationSettings settings,
    ITokenManager tokenManager)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
    _sessionQuerier = sessionQuerier;
    _settings = settings;
    _tokenManager = tokenManager;
  }

  public async Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken)
  {
    string baseUrl = _httpContextAccessor.HttpContext?.GetBaseUri().ToString() ?? throw new InvalidOperationException("The HttpContext is required."); // TODO(fpion): refactor
    Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache."); // TODO(fpion): refactor
    int expiresIn = _settings.AccessToken.LifetimeSeconds;

    ClaimsIdentity subject = session.CreateAccessTokenIdentity();
    CreatedToken access = await _tokenManager.CreateAsync(new CreateTokenParameters(subject, configuration.Secret)
    {
      Audience = baseUrl,
      Expires = DateTime.UtcNow.AddSeconds(expiresIn),
      Issuer = baseUrl,
      Type = _settings.AccessToken.Type
    }, cancellationToken);

    return new TokenResponse(access.TokenString, Schemes.Bearer)
    {
      ExpiresIn = expiresIn,
      RefreshToken = session.RefreshToken
    };
  }

  public async Task<Session> ValidateTokenAsync(string token, CancellationToken cancellationToken)
  {
    string baseUrl = _httpContextAccessor.HttpContext?.GetBaseUri().ToString() ?? throw new InvalidOperationException("The HttpContext is required."); // TODO(fpion): refactor
    Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache."); // TODO(fpion): refactor

    ValidatedToken validatedToken = await _tokenManager.ValidateAsync(new ValidateTokenParameters(token, configuration.Secret)
    {
      ValidAudiences = [baseUrl],
      ValidIssuers = [baseUrl],
      ValidTypes = [_settings.AccessToken.Type]
    }, cancellationToken);

    Claim[] claims = (validatedToken.ClaimsPrincipal.FindAll(Rfc7519ClaimNames.SessionId)).ToArray();
    if (claims.Length != 1)
    {
      throw new InvalidOperationException($"The access token did contain {claims.Length} session identifier claims."); // TODO(fpion): implement
    }

    Guid sessionId = Guid.Parse(claims.Single().Value);
    return await _sessionQuerier.ReadAsync(sessionId, cancellationToken) ?? throw new InvalidOperationException($"The session 'Id={sessionId}' could not be found."); // TODO(fpion): refactor
  }
}
