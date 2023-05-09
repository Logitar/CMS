namespace Logitar.Cms.Web.Models.Account;

public record AccountSignInInput
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool Remember { get; set; }
}
