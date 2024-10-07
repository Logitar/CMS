using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class TextPropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyTextProperties Properties { get; }

  public TextPropertiesChangedEvent(ReadOnlyTextProperties properties)
  {
    Properties = properties;
  }
}
