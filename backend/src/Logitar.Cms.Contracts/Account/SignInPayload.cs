namespace Logitar.Cms.Contracts.Account;

public record SignInPayload
{
  public string Username { get; set; }
  public string Password { get; set; }

  public SignInPayload() : this(string.Empty, string.Empty)
  {
  }

  public SignInPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
