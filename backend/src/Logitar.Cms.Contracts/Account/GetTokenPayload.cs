namespace Logitar.Cms.Contracts.Account;

public record GetTokenPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  public SignInPayload? Credentials { get; set; }
}
