using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Events;

public record ContentTypeCreatedEvent : DomainEvent, INotification
{
  public bool IsInvariant { get; }

  public IdentifierUnit UniqueName { get; }

  public ContentTypeCreatedEvent(bool isInvariant, IdentifierUnit uniqueName, ActorId actorId)
  {
    IsInvariant = isInvariant;
    UniqueName = uniqueName;
    ActorId = actorId;
  }
}
