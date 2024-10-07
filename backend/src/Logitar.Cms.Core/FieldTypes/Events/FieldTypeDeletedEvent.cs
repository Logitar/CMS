using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class FieldTypeDeletedEvent : DomainEvent, INotification;
