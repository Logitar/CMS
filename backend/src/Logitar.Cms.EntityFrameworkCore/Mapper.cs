using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Roles;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
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
      UniqueNameSettings = new(source.UniqueNameSettings),
      PasswordSettings = new(source.PasswordSettings),
      RequireUniqueEmail = source.RequireUniqueEmail,
      LoggingSettings = new(source.LoggingSettings)
    };

    MapAggregate(source, destination);

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
      // TODO(fpion): fill
    };

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
