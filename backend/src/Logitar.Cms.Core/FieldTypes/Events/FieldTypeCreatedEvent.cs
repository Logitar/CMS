using Logitar.Cms.Contracts.FieldTypes;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

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
