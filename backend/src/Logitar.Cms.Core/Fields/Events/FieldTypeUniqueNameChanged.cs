using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeUniqueNameChanged(UniqueName UniqueName) : DomainEvent, INotification;
