using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null;
}
