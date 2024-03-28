using Logitar.Cms.Contracts.Fields;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeCreatedEvent(UniqueNameUnit UniqueName, DataType DataType) : DomainEvent, INotification;
