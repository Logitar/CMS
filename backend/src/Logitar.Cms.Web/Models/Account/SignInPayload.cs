using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Extensions;

namespace Logitar.Cms.Web.Models.Account;

public record SignInPayload : Credentials
{
  public SignInPayload() : base()
  {
  }

  public SignInPayload(string username, string password) : base(username, password)
  {
  }

  public SignInSessionPayload ToPayload(HttpContext context) => new(Username, Password, context.GetSessionCustomAttributes());
}
