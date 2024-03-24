namespace Logitar.Cms.Contracts.Sessions;

public record SignInSessionPayload
{
  public string Username { get; set; }
  public string Password { get; set; }

  public string? IpAddress { get; set; }
  public string? AdditionalInformation { get; set; }

  public SignInSessionPayload() : this(string.Empty, string.Empty)
  {
  }

  public SignInSessionPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
