using Logitar.Cms.Core.ContentTypes;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentCreatedEvent : DomainEvent, INotification
{
  public ContentTypeId ContentTypeId { get; }

  public UniqueNameUnit UniqueName { get; }

  public ContentCreatedEvent(ContentTypeId contentTypeId, UniqueNameUnit uniqueName, ActorId actorId)
  {
    ContentTypeId = contentTypeId;
    UniqueName = uniqueName;
    ActorId = actorId;
  }
}
