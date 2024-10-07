namespace Logitar.Cms.Contracts.Sessions;

public record RenewSessionPayload
{
  [JsonPropertyName("refresh_token")]
  public string RefreshToken { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public RenewSessionPayload() : this(string.Empty)
  {
  }

  public RenewSessionPayload(string refreshToken, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    RefreshToken = refreshToken;

    CustomAttributes = customAttributes?.ToList() ?? [];
  }
}
