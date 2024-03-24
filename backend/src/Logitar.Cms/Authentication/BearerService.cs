using Logitar.Cms.Constants;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Models.Account;
using Logitar.Cms.Settings;
using Logitar.Cms.Web.Extensions;
using Logitar.Identity.Domain.Tokens;
using Logitar.Security.Claims;
using System.Security.Claims;

namespace Logitar.Cms.Authentication;

internal class BearerService : IBearerService
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly BearerSettings _settings;
  private readonly ITokenManager _tokenManager;

  public BearerService(ICacheService cacheService, IHttpContextAccessor httpContextAccessor, BearerSettings settings, ITokenManager tokenManager)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
    _settings = settings;
    _tokenManager = tokenManager;
  }

  public async Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken)
  {
    ClaimsIdentity subject = new();
    subject.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

    User user = session.User;
    subject.AddClaim(new(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    subject.AddClaim(new(Rfc7519ClaimNames.Username, user.Username));

    if (user.Email != null)
    {
      subject.AddClaim(new(Rfc7519ClaimNames.EmailAddress, user.Email.Address));
      subject.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, user.Email.IsVerified.ToString(), ClaimValueTypes.Boolean));
    }

    if (user.FullName != null)
    {
      if (user.FirstName != null)
      {
        subject.AddClaim(new(Rfc7519ClaimNames.FirstName, user.FirstName));
      }
      if (user.LastName != null)
      {
        subject.AddClaim(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
      subject.AddClaim(new(Rfc7519ClaimNames.FullName, user.FullName));
    }

    if (user.Picture != null)
    {
      subject.AddClaim(new(Rfc7519ClaimNames.Picture, user.Picture));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      subject.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value));
    }
    subject.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.UpdatedAt, user.UpdatedOn));

    if (user.Roles.Count > 0)
    {
      string roles = string.Join(' ', user.Roles.Select(role => role.UniqueName));
      subject.AddClaim(new(Rfc7519ClaimNames.Roles, roles));
    }

    Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
    CreateTokenParameters parameters = new(subject, configuration.Secret, new CreateTokenOptions
    {
      Type = _settings.TokenType,
      Expires = DateTime.UtcNow.AddSeconds(_settings.LifetimeSeconds)
    });
    if (_httpContextAccessor.HttpContext != null)
    {
      Uri baseUri = _httpContextAccessor.HttpContext.GetBaseUri();
      parameters.Audience = baseUri.ToString();
      parameters.Issuer = baseUri.ToString();
    }
    CreatedToken createdToken = await _tokenManager.CreateAsync(parameters, cancellationToken);

    TokenResponse response = new(createdToken.TokenString, Schemes.Bearer)
    {
      ExpiresIn = _settings.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
    return response;
  }
}
