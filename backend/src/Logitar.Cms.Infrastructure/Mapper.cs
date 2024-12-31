using Logitar.Cms.Core;
using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.Infrastructure;

public class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];
  private readonly ActorModel _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (ActorModel actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static ActorModel ToActor(ActorEntity actor) => new()
  {
    Id = new ActorId(actor.Id).ToGuid(),
    Type = Enum.Parse<ActorType>(actor.Type),
    IsDeleted = actor.IsDeleted,
    DisplayName = actor.DisplayName,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public LanguageModel ToLanguage(LanguageEntity source)
  {
    LanguageModel destination = new()
    {
      IsDefault = source.IsDefault,
      Locale = new LocaleModel(source.Code)
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Id = new StreamId(source.StreamId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private ActorModel? TryFindActor(string? id) => TryFindActor(id == null ? null : new ActorId(id));
  private ActorModel? TryFindActor(ActorId? actorId)
  {
    if (actorId.HasValue)
    {
      return _actors.TryGetValue(actorId.Value, out ActorModel? actor) ? actor : null;
    }

    return null;
  }
}
