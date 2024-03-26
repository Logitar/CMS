using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Events;

public record ContentTypeCreatedEvent(bool IsInvariant, IdentifierUnit UniqueName) : DomainEvent, INotification;
