using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record TextFieldTypePropertiesChangedEvent(ReadOnlyTextProperties Properties) : DomainEvent, INotification;
