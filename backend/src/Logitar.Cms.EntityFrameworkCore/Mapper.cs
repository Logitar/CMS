using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Roles;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.EntityFrameworkCore.Entities;
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

  public ApiKey ToApiKey(ApiKeyEntity source)
  {
    ApiKey destination = new(source.DisplayName)
    {
      Description = source.Description,
      ExpiresOn = source.ExpiresOn?.AsUniversalTime(),
      AuthenticatedOn = source.AuthenticatedOn?.AsUniversalTime()
    };

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role));
    }

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Configuration ToConfiguration(ConfigurationAggregate source)
  {
    Configuration destination = new(source.Secret.Value)
    {
      UniqueNameSettings = new(source.UniqueNameSettings),
      PasswordSettings = new(source.PasswordSettings),
      RequireUniqueEmail = source.RequireUniqueEmail,
      LoggingSettings = new(source.LoggingSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public CmsContentType ToContentType(ContentTypeEntity source)
  {
    CmsContentType destination = new(source.UniqueName)
    {
      IsInvariant = source.IsInvariant,
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public FieldType ToFieldType(FieldTypeEntity source)
  {
    FieldType destination = new(source.UniqueName)
    {
      DisplayName = source.DisplayName,
      Description = source.Description,
      DataType = source.DataType
    };

    MapAggregate(source, destination);

    switch (destination.DataType)
    {
      case DataType.String:
        destination.StringProperties = GetStringProperties(source.Properties);
        break;
      default:
        throw new DataTypeNotSupportedException(destination.DataType);
    }

    return destination;
  }
  private static StringProperties GetStringProperties(IReadOnlyDictionary<string, string> source)
  {
    StringProperties destination = new();
    if (source.TryGetValue(nameof(IStringProperties.MinimumLength), out string? minimumLengthValue)
      && int.TryParse(minimumLengthValue, out int minimumLength))
    {
      destination.MinimumLength = minimumLength;
    }
    if (source.TryGetValue(nameof(IStringProperties.MaximumLength), out string? maximumLengthValue)
      && int.TryParse(maximumLengthValue, out int maximumLength))
    {
      destination.MaximumLength = maximumLength;
    }
    if (source.TryGetValue(nameof(IStringProperties.Pattern), out string? pattern))
    {
      destination.Pattern = pattern;
    }
    return destination;
  }

  public Language ToLanguage(LanguageEntity source)
  {
    Language destination = new(new Locale(source.Code))
    {
      IsDefault = source.IsDefault
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

    Session destination = new(ToUser(source.User))
    {
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryFindActor(source.SignedOutBy),
      SignedOutOn = source.SignedOutOn?.AsUniversalTime(),
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
      PasswordChangedOn = source.PasswordChangedOn?.AsUniversalTime(),
      DisabledBy = TryFindActor(source.DisabledBy),
      DisabledOn = source.DisabledOn?.AsUniversalTime(),
      IsDisabled = source.IsDisabled,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = source.Birthdate?.AsUniversalTime(),
      Gender = source.Gender,
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      AuthenticatedOn = source.AuthenticatedOn?.AsUniversalTime()
    };

    if (source.AddressStreet != null && source.AddressLocality != null && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new Address(source.AddressStreet, source.AddressLocality, source.AddressPostalCode, source.AddressRegion, source.AddressCountry, source.AddressFormatted)
      {
        IsVerified = source.IsAddressVerified,
        VerifiedBy = TryFindActor(source.AddressVerifiedBy),
        VerifiedOn = source.AddressVerifiedOn?.AsUniversalTime()
      };
    }
    if (source.EmailAddress != null)
    {
      destination.Email = new Email(source.EmailAddress)
      {
        IsVerified = source.IsEmailVerified,
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = source.EmailVerifiedOn?.AsUniversalTime()
      };
    }
    if (source.PhoneNumber != null && source.PhoneE164Formatted != null)
    {
      destination.Phone = new Phone(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted)
      {
        IsVerified = source.IsPhoneVerified,
        VerifiedBy = TryFindActor(source.PhoneVerifiedBy),
        VerifiedOn = source.PhoneVerifiedOn?.AsUniversalTime()
      };
    }

    if (source.Locale != null)
    {
      destination.Locale = new Locale(source.Locale);
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role));
    }

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    foreach (UserIdentifierEntity identifier in source.Identifiers)
    {
      destination.CustomIdentifiers.Add(new CustomIdentifier(identifier.Key, identifier.Value));
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, Aggregate destination)
  {
    destination.Id = source.Id.ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }
  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : Actor.System;
  private Actor? TryFindActor(string? id) => id == null ? null : FindActor(id);
}
