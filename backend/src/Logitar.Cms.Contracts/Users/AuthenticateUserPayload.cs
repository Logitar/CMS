namespace Logitar.Cms.Contracts.Users;

public record AuthenticateUserPayload
{
  public string Username { get; set; }
  public string Password { get; set; }

  public AuthenticateUserPayload() : this(string.Empty, string.Empty)
  {
  }

  public AuthenticateUserPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
