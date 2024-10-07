using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];

  public Mapper() : this([])
  {
  }

  public Mapper(IEnumerable<Actor> actors)
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static Actor ToActor(ActorEntity actor) => new(actor.DisplayName)
  {
    Id = new ActorId(actor.Id).ToGuid(),
    Type = Enum.Parse<ActorType>(actor.Type),
    IsDeleted = actor.IsDeleted,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public LanguageModel ToLanguage(LanguageEntity source)
  {
    LanguageModel destination = new()
    {
      IsDefault = source.IsDefault,
      Locale = new LocaleModel(source.Locale)
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, AggregateModel destination)
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
}
