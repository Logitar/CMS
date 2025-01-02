using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeCreated(UniqueName UniqueName, DataType DataType) : DomainEvent, INotification;
