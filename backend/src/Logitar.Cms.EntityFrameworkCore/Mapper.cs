using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.ContentTypes;
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

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : Actor.System;

  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Local => value.ToUniversalTime(),
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    _ => value,
  };
}
