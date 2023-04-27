using Logitar.Cms.Contracts.Users;
using System.Security.Claims;

namespace Logitar.Cms.Core.Claims;

public static class ClaimExtensions
{
  public static ClaimsIdentity GetClaimsIdentity(this User user, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimTypes.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimTypes.Username, user.Username));
    identity.AddClaim(user.UpdatedOn.GetClaim(Rfc7519ClaimTypes.UpdatedOn));

    if (user.Email != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.EmailAddress, user.Email.Address));
      identity.AddClaim(new(Rfc7519ClaimTypes.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }

    if (user.FullName != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.FullName, user.FullName));

      if (user.FirstName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.FirstName, user.FirstName));
      }

      if (user.LastName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.LastName, user.LastName));
      }
    }

    if (user.Locale != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Locale, user.Locale));
    }

    if (user.Picture != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Picture, user.Picture));
    }

    if (user.SignedInOn.HasValue)
    {
      identity.AddClaim(user.SignedInOn.Value.GetClaim(Rfc7519ClaimTypes.AuthenticationTime));
    }

    return identity;
  }

  internal static Claim GetClaim(this DateTime moment, string type)
  {
    return new(type, new DateTimeOffset(moment).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);
  }
}
