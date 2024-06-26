﻿using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Web.Models.Account;

public record UserProfile
{
  public DateTime CreatedOn { get; set; }
  public DateTime UpdatedOn { get; set; }

  public string Username { get; set; }
  public DateTime? PasswordChangedOn { get; set; }
  public DateTime? AuthenticatedOn { get; set; }

  public Address? Address { get; set; }
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }

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

  public UserProfile() : this(string.Empty)
  {
  }

  public UserProfile(string username)
  {
    Username = username;
  }

  public UserProfile(User user) : this(user.UniqueName)
  {
    CreatedOn = user.CreatedOn;
    UpdatedOn = user.UpdatedOn;

    PasswordChangedOn = user.PasswordChangedOn;
    AuthenticatedOn = user.AuthenticatedOn;

    Address = user.Address;
    Email = user.Email;
    Phone = user.Phone;

    FirstName = user.FirstName;
    MiddleName = user.MiddleName;
    LastName = user.LastName;
    FullName = user.FullName;
    Nickname = user.Nickname;

    Birthdate = user.Birthdate;
    Gender = user.Gender;
    Locale = user.Locale;
    TimeZone = user.TimeZone;

    Picture = user.Picture;
    Profile = user.Profile;
    Website = user.Website;
  }
}
