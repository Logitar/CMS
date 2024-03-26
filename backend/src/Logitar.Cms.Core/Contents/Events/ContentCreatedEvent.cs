using Logitar.Cms.Core.ContentTypes;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentCreatedEvent(ContentTypeId ContentTypeId, ContentLocaleUnit Invariant) : DomainEvent, INotification;
