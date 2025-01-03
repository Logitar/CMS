﻿using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Roles.Models;

namespace Logitar.Cms.Core.Users.Models;

public class UserModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;

  public bool HasPassword { get; set; }
  public ActorModel? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public ActorModel? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

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

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
  public List<CustomIdentifierModel> CustomIdentifiers { get; set; } = [];
  public List<RoleModel> Roles { get; set; } = [];

  public UserModel()
  {
  }

  public UserModel(string uniqueName) : this()
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
