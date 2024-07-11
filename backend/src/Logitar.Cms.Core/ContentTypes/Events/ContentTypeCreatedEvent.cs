using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Events;

public class ContentTypeCreatedEvent : DomainEvent, INotification
{
  public bool IsInvariant { get; }

  public IdentifierUnit UniqueName { get; }

  public ContentTypeCreatedEvent(bool isInvariant, IdentifierUnit uniqueName)
  {
    IsInvariant = isInvariant;

    UniqueName = uniqueName;
  }
}
