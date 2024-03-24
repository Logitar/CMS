namespace Logitar.Cms.Models.Account;

public record GetTokenPayload
{
  public string? RefreshToken { get; set; }

  public string? Username { get; set; }
  public string? Password { get; set; }
}
