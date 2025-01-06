namespace Logitar.Cms.Web.Models.Account;

public record GetTokenPayload : SignInPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }
}
