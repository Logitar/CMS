using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class BooleanFieldTypePropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyBooleanProperties Properties { get; }

  public BooleanFieldTypePropertiesChangedEvent(ReadOnlyBooleanProperties properties)
  {
    Properties = properties;
  }
}
