using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentCreated(ContentTypeId ContentTypeId, ContentLocale Invariant) : DomainEvent, INotification;
