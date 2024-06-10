using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class NumberFieldTypePropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyNumberProperties Properties { get; }

  public NumberFieldTypePropertiesChangedEvent(ReadOnlyNumberProperties properties)
  {
    Properties = properties;
  }
}
