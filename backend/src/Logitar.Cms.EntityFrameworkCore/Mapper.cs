using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Contracts.Roles;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
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

  public ContentItem ToContentItem(ContentItemEntity source, ContentType? contentType = null)
  {
    if (contentType == null)
    {
      if (source.ContentType == null)
      {
        throw new ArgumentException($"The {nameof(source.ContentType)} is required.", nameof(source));
      }
      contentType = ToContentType(source.ContentType);
    }

    ContentItem destination = new(contentType);

    foreach (ContentLocaleEntity locale in source.ContentLocales)
    {
      destination.Locales.Add(ToContentLocale(locale, destination));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public ContentLocale ToContentLocale(ContentLocaleEntity source, ContentItem? contentItem = null)
  {
    if (contentItem == null)
    {
      if (source.ContentItem == null)
      {
        throw new ArgumentException($"The {nameof(source.ContentItem)} is required.", nameof(source));
      }
      contentItem = ToContentItem(source.ContentItem);
    }

    ContentLocale destination = new(contentItem, source.UniqueName)
    {
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    return destination;
  }

  public ContentType ToContentType(ContentTypeEntity source)
  {
    ContentType destination = new(source.UniqueName)
    {
      IsInvariant = source.IsInvariant,
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Language ToLanguage(LanguageEntity source)
  {
    Language destination = new(new Locale(source.Locale))
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

    MapAggregate(source, destination);

    return destination;
  }

  public Session ToSession(SessionEntity source, User? user = null)
  {
    if (user == null)
    {
      if (source.User == null)
      {
        throw new ArgumentException($"The {nameof(source.User)} is required.", nameof(source));
      }
      user = ToUser(source.User);
    }

    Session destination = new(user)
    {
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryFindActor(source.SignedOutBy),
      SignedOutOn = AsUniversalTime(source.SignedOutOn)
    };
    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      switch (customAttribute.Key)
      {
        case "AdditionalInformation":
          destination.AdditionalInformation = customAttribute.Value;
          break;
        case "IpAddress":
          destination.IpAddress = customAttribute.Value;
          break;
      }
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
      LastName = source.LastName,
      FullName = source.FullName,
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      AuthenticatedOn = AsUniversalTime(source.AuthenticatedOn)
    };
    if (source.Locale != null)
    {
      destination.Locale = new Locale(source.Locale);
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
