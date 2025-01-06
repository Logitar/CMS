namespace Logitar.Cms.Web.Models.Account;

public record SignInPayload
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
