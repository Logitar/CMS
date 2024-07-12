using Logitar.Cms.Core.ContentTypes;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public class ContentCreatedEvent : DomainEvent, INotification
{
  public ContentTypeId ContentTypeId { get; }

  public ContentLocaleUnit Invariant { get; }

  public ContentCreatedEvent(ContentTypeId contentTypeId, ContentLocaleUnit invariant)
  {
    ContentTypeId = contentTypeId;

    Invariant = invariant;
  }
}
