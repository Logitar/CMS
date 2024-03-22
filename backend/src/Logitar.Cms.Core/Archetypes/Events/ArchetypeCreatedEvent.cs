using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Events;

public record ArchetypeCreatedEvent : DomainEvent, INotification
{
  public IdentifierUnit UniqueName { get; }

  public ArchetypeCreatedEvent(IdentifierUnit uniqueName, ActorId actorId)
  {
    UniqueName = uniqueName;
    ActorId = actorId;
  }
}
