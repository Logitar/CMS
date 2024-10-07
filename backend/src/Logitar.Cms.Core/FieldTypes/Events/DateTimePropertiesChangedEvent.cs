using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class DateTimePropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyDateTimeProperties Properties { get; }

  public DateTimePropertiesChangedEvent(ReadOnlyDateTimeProperties properties)
  {
    Properties = properties;
  }
}
