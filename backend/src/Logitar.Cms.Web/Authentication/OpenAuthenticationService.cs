using Logitar.Cms.Core.Roles.Models;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Users.Models;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Models.Account;
using Logitar.Cms.Web.Settings;
using Logitar.Identity.Core.Tokens;
using Logitar.Security.Claims;

namespace Logitar.Cms.Web.Authentication;

internal class OpenAuthenticationService : IOpenAuthenticationService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly OpenAuthenticationSettings _settings;
  private readonly ITokenManager _tokenManager;

  public OpenAuthenticationService(
    IHttpContextAccessor httpContextAccessor,
    ISessionQuerier sessionQuerier,
    OpenAuthenticationSettings settings,
    ITokenManager tokenManager)
  {
    _httpContextAccessor = httpContextAccessor;
    _sessionQuerier = sessionQuerier;
    _settings = settings;
    _tokenManager = tokenManager;
  }

  public async Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken)
  {
    AccessTokenSettings settings = _settings.AccessToken;

    string baseUrl = GetBaseUrl();
    ValidateTokenParameters parameters = new(accessToken, settings.Secret)
    {
      ValidTypes = [settings.TokenType],
      ValidAudiences = [baseUrl],
      ValidIssuers = [baseUrl]
    };

    ValidatedToken validatedToken = await _tokenManager.ValidateAsync(parameters, cancellationToken);

    Claim claim = validatedToken.ClaimsPrincipal.FindAll(Rfc7519ClaimNames.SessionId).Single();
    Guid sessionId = Guid.Parse(claim.Value);
    return await _sessionQuerier.ReadAsync(sessionId, cancellationToken)
      ?? throw new ArgumentException($"The session 'Id={sessionId}' could not be found.", nameof(accessToken));
  }

  public async Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken)
  {
    AccessTokenSettings settings = _settings.AccessToken;

    ClaimsIdentity subject = CreateClaimsIdentity(session);
    string baseUrl = GetBaseUrl();
    DateTime now = DateTime.Now;
    DateTime expires = now.AddSeconds(settings.LifetimeSeconds);
    CreateTokenParameters parameters = new(subject, settings.Secret)
    {
      Type = settings.TokenType,
      Audience = baseUrl,
      Issuer = baseUrl,
      Expires = expires,
      IssuedAt = now,
      NotBefore = now
    };

    CreatedToken createdToken = await _tokenManager.CreateAsync(parameters, cancellationToken);

    return new TokenResponse
    {
      AccessToken = createdToken.TokenString,
      TokenType = Schemes.Bearer,
      ExpiresIn = settings.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
  }
  private static ClaimsIdentity CreateClaimsIdentity(SessionModel session)
  {
    UserModel user = session.User;

    ClaimsIdentity identity = new(Schemes.Bearer);

    identity.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

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

    if (user.Picture != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Picture, user.Picture));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value));
    }

    foreach (RoleModel role in user.Roles)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Roles, role.UniqueName));
    }

    return identity;
  }

  private string GetBaseUrl()
  {
    if (_httpContextAccessor.HttpContext == null)
    {
      throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");
    }

    HttpRequest request = _httpContextAccessor.HttpContext.Request;
    Uri baseUri = new($"{request.Scheme}://{request.Host}", UriKind.Absolute);
    return baseUri.ToString();
  }
}
