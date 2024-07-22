using Logitar.Cms.Contracts;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Events;

public class FieldTypeUpdatedEvent : DomainEvent, INotification
{
  public UniqueNameUnit? UniqueName { get; set; }
  public Change<DisplayNameUnit>? DisplayName { get; set; }
  public Change<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => UniqueName != null || DisplayName != null || Description != null;
}
