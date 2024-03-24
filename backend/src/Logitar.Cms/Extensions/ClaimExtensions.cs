using Logitar.Cms.Contracts.Roles;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Security.Claims;

namespace Logitar.Cms.Extensions;

internal static class ClaimsExtensions
{
  public static ClaimsIdentity CreateClaimsIdentity(this Session session, string? authenticationType = null)
  {
    ClaimsIdentity identity = session.User.CreateClaimsIdentity(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

    return identity;
  }
  public static ClaimsIdentity CreateClaimsIdentity(this User user, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.Username, user.Username));
    identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.UpdatedAt, user.UpdatedOn));

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

      if (user.LastName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
    }

    if (user.Locale != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Locale, user.Locale.Code));
    }
    if (user.TimeZone != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.TimeZone, user.TimeZone));
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

    return identity;
  }
}
