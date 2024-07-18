using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class BooleanPropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyBooleanProperties Properties { get; }

  public BooleanPropertiesChangedEvent(ReadOnlyBooleanProperties properties)
  {
    Properties = properties;
  }
}
