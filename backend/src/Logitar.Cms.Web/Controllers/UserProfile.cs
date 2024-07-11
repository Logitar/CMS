using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Web.Controllers;

public record UserProfile
{
  public string Username { get; set; }

  public DateTime? PasswordChangedOn { get; set; }

  public string? EmailAddress { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public Locale? Locale { get; set; }

  public string? Picture { get; set; }

  public DateTime CreatedOn { get; set; }
  public DateTime UpdatedOn { get; set; }
  public DateTime AuthenticatedOn { get; set; }

  public UserProfile() : this(string.Empty)
  {
  }

  public UserProfile(string username)
  {
    Username = username;
  }

  public UserProfile(User user) : this(user.UniqueName)
  {
    PasswordChangedOn = user.PasswordChangedOn;

    EmailAddress = user.Email?.Address;

    FirstName = user.FirstName;
    MiddleName = user.MiddleName;
    LastName = user.LastName;
    FullName = user.FullName;

    Locale = user.Locale;

    Picture = user.Picture;

    CreatedOn = user.CreatedOn;
    UpdatedOn = user.UpdatedOn;
    AuthenticatedOn = user.AuthenticatedOn ?? user.UpdatedOn;
  }
}
