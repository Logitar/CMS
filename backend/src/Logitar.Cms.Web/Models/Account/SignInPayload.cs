using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Extensions;

namespace Logitar.Cms.Web.Models.Account;

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

  public SignInSessionPayload ToPayload(HttpContext context) => new(Username, Password, context.GetSessionCustomAttributes());
}
