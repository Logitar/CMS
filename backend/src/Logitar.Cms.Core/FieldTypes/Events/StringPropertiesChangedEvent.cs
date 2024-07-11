using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class StringPropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyStringProperties Properties { get; }

  public StringPropertiesChangedEvent(ReadOnlyStringProperties properties)
  {
    Properties = properties;
  }
}
