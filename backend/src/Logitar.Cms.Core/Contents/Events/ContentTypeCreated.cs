using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentTypeCreated(bool IsInvariant, Identifier UniqueName) : DomainEvent, INotification;
