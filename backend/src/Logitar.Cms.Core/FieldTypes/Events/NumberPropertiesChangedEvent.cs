using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class NumberPropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyNumberProperties Properties { get; }

  public NumberPropertiesChangedEvent(ReadOnlyNumberProperties properties)
  {
    Properties = properties;
  }
}
