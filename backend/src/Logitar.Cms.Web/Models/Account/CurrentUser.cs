using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Web.Models.Account;

public record CurrentUser
{
  public string DisplayName { get; set; } = string.Empty;
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public CurrentUser()
  {
  }

  public CurrentUser(SessionModel session) : this(session.User)
  {
  }

  public CurrentUser(UserModel user)
  {
    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }
}
