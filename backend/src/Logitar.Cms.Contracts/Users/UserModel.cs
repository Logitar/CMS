using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Roles;

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

  public AddressModel? Address { get; set; }
  public EmailModel? Email { get; set; }
  public PhoneModel? Phone { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public LocaleModel? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }
  public List<CustomIdentifier> CustomIdentifiers { get; set; }
  public List<RoleModel> Roles { get; set; }

  public UserModel() : this(string.Empty)
  {
  }

  public UserModel(string username)
  {
    Username = username;

    CustomAttributes = [];
    CustomIdentifiers = [];
    Roles = [];
  }

  public override string ToString() => $"{FullName ?? Username} | {base.ToString()}";
}
