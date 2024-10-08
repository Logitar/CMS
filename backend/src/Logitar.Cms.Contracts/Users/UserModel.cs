using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts.Users;

public class UserModel : AggregateModel
{
  public string Username { get; set; }

  public bool HasPassword { get; set; }
  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public bool IsDisabled { get; set; }
  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }

  public EmailModel? Email { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public string? PictureUrl { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public UserModel() : this(string.Empty)
  {
  }

  public UserModel(string username)
  {
    Username = username;
  }
}
