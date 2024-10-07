using Logitar.Cms.Contracts;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Events;

public class ContentTypeUpdatedEvent : DomainEvent, INotification
{
  public Change<DisplayNameUnit>? DisplayName { get; set; }
  public Change<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => DisplayName != null || Description != null;
}
