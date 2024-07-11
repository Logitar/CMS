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

  private string? _baseUrl = null;
  private string BaseUrl
  {
    get
    {
      if (_baseUrl == null)
      {
        HttpContext context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");
        _baseUrl = context.GetBaseUri().ToString();
      }
      return _baseUrl;
    }
  }

  private Configuration Configuration => _cacheService.GetConfiguration();

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
    ClaimsIdentity subject = session.CreateAccessTokenIdentity();
    int expiresIn = _settings.AccessToken.LifetimeSeconds;
    CreatedToken access = await _tokenManager.CreateAsync(new CreateTokenParameters(subject, Configuration.Secret)
    {
      Audience = BaseUrl,
      Expires = DateTime.UtcNow.AddSeconds(expiresIn),
      Issuer = BaseUrl,
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
    ValidatedToken validatedToken = await _tokenManager.ValidateAsync(new ValidateTokenParameters(token, Configuration.Secret)
    {
      ValidAudiences = [BaseUrl],
      ValidIssuers = [BaseUrl],
      ValidTypes = [_settings.AccessToken.Type]
    }, cancellationToken);

    IEnumerable<Claim> claims = validatedToken.ClaimsPrincipal.FindAll(Rfc7519ClaimNames.SessionId);
    int count = claims.Count();
    var sessionId = count switch
    {
      0 => throw new InvalidOperationException($"The access token did not contain any '{Rfc7519ClaimNames.SessionId}' claim."),
      1 => Guid.Parse(claims.Single().Value),
      _ => throw new InvalidOperationException($"The access token did contain many ({count}) '{Rfc7519ClaimNames.SessionId}' claims."),
    };
    return await _sessionQuerier.ReadAsync(sessionId, cancellationToken) ?? throw new InvalidOperationException($"The session 'Id={sessionId}' could not be found.");
  }
}
