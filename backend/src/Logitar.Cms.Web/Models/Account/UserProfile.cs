using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Web.Models.Account;

public record UserProfile
{
  public string Username { get; set; } = string.Empty;
  public DateTime? PasswordChangedOn { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public DateTime CreatedOn { get; set; }
  public DateTime UpdatedOn { get; set; }
  public DateTime? AuthenticatedOn { get; set; }

  public UserProfile()
  {
  }

  public UserProfile(UserModel user)
  {
    Username = user.UniqueName;
    PasswordChangedOn = user.PasswordChangedOn;

    FirstName = user.FirstName;
    MiddleName = user.MiddleName;
    LastName = user.LastName;
    FullName = user.FullName;

    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;

    CreatedOn = user.CreatedOn;
    UpdatedOn = user.UpdatedOn;
    AuthenticatedOn = user.AuthenticatedOn;
  }
}
