using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Events;

public class FieldDefinitionChangedEvent : DomainEvent, INotification
{
  public Guid FieldId { get; }
  public FieldDefinitionUnit FieldDefinition { get; }
  public int? Order { get; }

  public FieldDefinitionChangedEvent(Guid fieldId, FieldDefinitionUnit fieldDefinition, int? order)
  {
    FieldId = fieldId;
    FieldDefinition = fieldDefinition;
    Order = order;
  }
}
