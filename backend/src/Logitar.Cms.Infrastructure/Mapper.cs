using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Models;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];
  private readonly ActorModel _system = ActorModel.System;

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (ActorModel actor in actors)
    {
      ActorId actorId = new(actor.Id);
      _actors[actorId] = actor;
    }
  }

  public static ActorModel ToActor(ActorEntity actor) => new(actor.DisplayName)
  {
    Id = actor.Id,
    Type = actor.Type,
    IsDeleted = actor.IsDeleted,
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
    try
    {
      destination.Id = new StreamId(source.StreamId).ToGuid();
    }
    catch (Exception)
    {
    }
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }
  private ActorModel FindActor(string? id)
  {
    if (id != null)
    {
      ActorId actorId = new(id);
      if (_actors.TryGetValue(actorId, out ActorModel? actor))
      {
        return actor;
      }
    }

    return _system;
  }
}
