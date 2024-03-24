namespace Logitar.Cms.Contracts.Sessions;

public record RenewSessionPayload
{
  public string RefreshToken { get; set; }

  public string? IpAddress { get; set; }
  public string? AdditionalInformation { get; set; }

  public RenewSessionPayload() : this(string.Empty)
  {
  }

  public RenewSessionPayload(string refreshToken)
  {
    RefreshToken = refreshToken;
  }
}
