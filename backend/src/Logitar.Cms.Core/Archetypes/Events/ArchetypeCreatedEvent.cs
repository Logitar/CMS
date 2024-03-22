using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Events;

public record ArchetypeCreatedEvent : DomainEvent, INotification
{
  public IdentifierUnit Identifier { get; }

  public ArchetypeCreatedEvent(IdentifierUnit identifier, ActorId actorId)
  {
    Identifier = identifier;
    ActorId = actorId;
  }
}
