using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public class FieldTypeUpdatedEvent : DomainEvent, INotification
{
  public UniqueNameUnit? UniqueName { get; set; }
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => UniqueName != null || DisplayName != null || Description != null;
}
