using Logitar.Cms.Contracts.Roles;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using Logitar.Cms.Web.Settings;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Tokens;
using Logitar.Security.Claims;

namespace Logitar.Cms.Web.Authentication;

public class OpenAuthenticationService : IOpenAuthenticationService
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly OpenAuthenticationSettings _settings;
  private readonly ITokenManager _tokenManager;

  public OpenAuthenticationService(ICacheService cacheService, IHttpContextAccessor httpContextAccessor,
    ISessionQuerier sessionQuerier, OpenAuthenticationSettings settings, ITokenManager tokenManager)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
    _sessionQuerier = sessionQuerier;
    _settings = settings;
    _tokenManager = tokenManager;
  }

  public virtual async Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken)
  {
    ClaimsIdentity subject = CreateClaimsIdentity(session);
    string secret = _cacheService.GetConfiguration().Secret;
    string baseUrl = GetBaseUrl();

    CreateTokenParameters parameters = new(subject, secret)
    {
      Type = _settings.AccessToken.TokenType,
      Audience = baseUrl,
      Issuer = baseUrl,
      Expires = DateTime.UtcNow.AddSeconds(_settings.AccessToken.Lifetime)
    };
    CreatedToken accessToken = await _tokenManager.CreateAsync(parameters, cancellationToken);

    TokenResponse tokenResponse = new(accessToken.TokenString, Schemes.Bearer)
    {
      ExpiresIn = _settings.AccessToken.Lifetime,
      RefreshToken = session.RefreshToken
    };
    return tokenResponse;
  }
  private static ClaimsIdentity CreateClaimsIdentity(Session session)
  {
    ClaimsIdentity identity = new();

    User user = session.User;
    identity.AddClaim(new(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.Username, user.UniqueName));
    if (user.Email != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.EmailAddress, user.Email.Address));
      identity.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.FullName != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.FullName, user.FullName));

      if (user.FirstName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.FirstName, user.FirstName));
      }

      if (user.MiddleName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.MiddleName, user.MiddleName));
      }

      if (user.LastName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
    }
    if (user.Locale != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Locale, user.Locale.Code));
    }
    if (user.Picture != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Picture, user.Picture));
    }
    if (user.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value));
    }
    foreach (Role role in user.Roles)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Roles, role.UniqueName));
    }

    identity.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

    return identity;
  }

  public async Task<Session> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken)
  {
    string secret = _cacheService.GetConfiguration().Secret;
    string baseUrl = GetBaseUrl();

    ValidateTokenParameters parameters = new(accessToken, secret)
    {
      ValidAudiences = [baseUrl],
      ValidIssuers = [baseUrl],
      ValidTypes = [_settings.AccessToken.TokenType]
    };
    ValidatedToken validatedToken = await _tokenManager.ValidateAsync(parameters, cancellationToken);

    Claim claim = validatedToken.ClaimsPrincipal.FindAll(Rfc7519ClaimNames.SessionId).Single();
    SessionId sessionId = new(Guid.Parse(claim.Value));
    return await _sessionQuerier.ReadAsync(sessionId, cancellationToken) ?? throw new SessionNotFoundException(sessionId, Rfc7519ClaimNames.SessionId);
  }

  private string GetBaseUrl()
  {
    HttpContext context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");
    return context.GetBaseUri().ToString();
  }
}
