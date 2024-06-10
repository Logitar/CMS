using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class TextFieldTypePropertiesChangedEvent : DomainEvent, INotification
{
  public ReadOnlyTextProperties Properties { get; }

  public TextFieldTypePropertiesChangedEvent(ReadOnlyTextProperties properties)
  {
    Properties = properties;
  }
}
