﻿using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Roles;

namespace Logitar.Cms.Contracts.Users;

public class User : Aggregate
{
  public string Username { get; set; }

  public bool HasPassword { get; set; }
  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public Email? Email { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public Locale? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<Role> Roles { get; set; }

  public User() : this(string.Empty)
  {
  }

  public User(string uniqueName)
  {
    Username = uniqueName;
    Roles = [];
  }

  public override string ToString() => $"{FullName ?? Username} | {base.ToString()}";
}
