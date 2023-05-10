using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Web.Models.Account;

public record CurrentUser
{
  public CurrentUser(User? user)
  {
    IsAuthenticated = user != null;

    if (user != null)
    {
      EmailAddress = user.Email?.Address;
      FullName = user.FullName;
      Picture = user.Picture;
    }
  }

  public bool IsAuthenticated { get; }

  public string? EmailAddress { get; }
  public string? FullName { get; }
  public string? Picture { get; }
}
