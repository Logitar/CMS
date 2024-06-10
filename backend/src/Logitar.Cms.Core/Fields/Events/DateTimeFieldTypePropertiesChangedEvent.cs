using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class DateTimeFieldTypePropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyDateTimeProperties Properties { get; }

  public DateTimeFieldTypePropertiesChangedEvent(ReadOnlyDateTimeProperties properties)
  {
    Properties = properties;
  }
}
