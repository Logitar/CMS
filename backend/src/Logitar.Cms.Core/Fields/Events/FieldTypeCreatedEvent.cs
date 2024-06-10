using Logitar.Cms.Contracts.Fields;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class FieldTypeCreatedEvent : DomainEvent, INotification
{
  public UniqueNameUnit UniqueName { get; }
  public DataType DataType { get; }

  public FieldTypeCreatedEvent(UniqueNameUnit uniqueName, DataType dataType)
  {
    UniqueName = uniqueName;
    DataType = dataType;
  }
}
