using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Events;

public record ArchetypeCreatedEvent : DomainEvent, INotification
{
  public bool IsInvariant { get; }

  public IdentifierUnit UniqueName { get; }

  public ArchetypeCreatedEvent(bool isInvariant, IdentifierUnit uniqueName, ActorId actorId)
  {
    IsInvariant = isInvariant;
    UniqueName = uniqueName;
    ActorId = actorId;
  }
}
