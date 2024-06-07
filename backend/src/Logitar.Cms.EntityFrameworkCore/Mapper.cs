using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Roles;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors;

  public Mapper()
  {
    _actors = [];
  }

  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static Actor ToActor(ActorEntity source) => new(source.DisplayName)
  {
    Id = new ActorId(source.Id).ToGuid(),
    Type = Enum.Parse<ActorType>(source.Type),
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public Configuration ToConfiguration(ConfigurationAggregate source)
  {
    Configuration destination = new(source.Secret.Value)
    {
      UniqueNameSettings = new UniqueNameSettings(source.UniqueNameSettings),
      PasswordSettings = new PasswordSettings(source.PasswordSettings),
      RequireUniqueEmail = source.RequireUniqueEmail,
      LoggingSettings = new LoggingSettings(source.LoggingSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Role ToRole(RoleEntity source)
  {
    Role destination = new(source.UniqueName)
    {
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Session ToSession(SessionEntity source)
  {
    if (source.User == null)
    {
      throw new ArgumentException($"The {nameof(source.User)} is required.", nameof(source));
    }

    User user = ToUser(source.User);
    Session destination = new(user)
    {
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryFindActor(source.SignedOutBy),
      SignedOutOn = AsUniversalTime(source.SignedOutOn)
    };

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public User ToUser(UserEntity source)
  {
    User destination = new(source.UniqueName)
    {
      HasPassword = source.HasPassword,
      PasswordChangedBy = TryFindActor(source.PasswordChangedBy),
      PasswordChangedOn = AsUniversalTime(source.PasswordChangedOn),
      DisabledBy = TryFindActor(source.DisabledBy),
      DisabledOn = AsUniversalTime(source.DisabledOn),
      IsDisabled = source.IsDisabled,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = AsUniversalTime(source.Birthdate),
      Gender = source.Gender,
      Locale = source.Locale == null ? null : new Locale(source.Locale),
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      AuthenticatedOn = AsUniversalTime(source.AuthenticatedOn)
    };

    if (source.AddressStreet != null && source.AddressLocality != null && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new Address(source.AddressStreet, source.AddressLocality, source.AddressPostalCode, source.AddressRegion, source.AddressCountry, source.AddressFormatted)
      {
        IsVerified = source.IsAddressVerified,
        VerifiedBy = TryFindActor(source.AddressVerifiedBy),
        VerifiedOn = AsUniversalTime(source.AddressVerifiedOn)
      };
    }
    if (source.EmailAddress != null)
    {
      destination.Email = new Email(source.EmailAddress)
      {
        IsVerified = source.IsEmailVerified,
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = AsUniversalTime(source.EmailVerifiedOn)
      };
    }
    if (source.PhoneNumber != null && source.PhoneE164Formatted != null)
    {
      destination.Phone = new Phone(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted)
      {
        IsVerified = source.IsEmailVerified,
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = AsUniversalTime(source.EmailVerifiedOn)
      };
    }

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    foreach (UserIdentifierEntity identifier in source.Identifiers)
    {
      destination.CustomIdentifiers.Add(new CustomIdentifier(identifier.Key, identifier.Value));
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role));
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }
  private void MapAggregate(AggregateRoot source, Aggregate destination)
  {
    destination.Id = source.Id.ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private Actor? TryFindActor(string? id) => id == null ? null : FindActor(id);
  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : Actor.System;

  private static DateTime? AsUniversalTime(DateTime? value) => value.HasValue ? AsUniversalTime(value.Value) : null;
  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Local => value.ToUniversalTime(),
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    _ => value,
  };
}
