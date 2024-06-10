using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Extensions;

namespace Logitar.Cms.Web.Models.Account;

public record GetTokenPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  public Credentials? Credentials { get; set; }

  public RenewSessionPayload ToRenewPayload(HttpContext context)
  {
    if (RefreshToken == null)
    {
      throw new InvalidOperationException($"The {nameof(RefreshToken)} is required.");
    }

    return new RenewSessionPayload(RefreshToken)
    {
      AdditionalInformation = context.GetAdditionalInformation(),
      IpAddress = context.GetClientIpAddress()
    };
  }

  public SignInSessionPayload ToSignInPayload(HttpContext context)
  {
    if (Credentials == null)
    {
      throw new InvalidOperationException($"The {nameof(Credentials)} is required.");
    }

    return new SignInSessionPayload(Credentials.Username, Credentials.Password)
    {
      AdditionalInformation = context.GetAdditionalInformation(),
      IpAddress = context.GetClientIpAddress()
    };
  }
}
