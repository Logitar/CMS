using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class StringFieldTypePropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyStringProperties Properties { get; }

  public StringFieldTypePropertiesChangedEvent(ReadOnlyStringProperties properties)
  {
    Properties = properties;
  }
}
