namespace Logitar.Cms.Core.Users.Models;

public record AuthenticateUserPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public AuthenticateUserPayload()
  {
  }

  public AuthenticateUserPayload(string uniqueName, string password)
  {
    UniqueName = uniqueName;
    Password = password;
  }
}
