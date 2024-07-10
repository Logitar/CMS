using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Roles;

namespace Logitar.Cms.Contracts.Users;

public class User : Aggregate
{
  public string UniqueName { get; set; }

  public bool HasPassword { get; set; }
  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public Address? Address { get; set; }
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public Locale? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }
  public List<CustomIdentifier> CustomIdentifiers { get; set; }
  public List<Role> Roles { get; set; }

  public User() : this(string.Empty)
  {
  }

  public User(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
    CustomIdentifiers = [];
    Roles = [];
  }

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
