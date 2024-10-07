using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Events;

public class ContentTypeDeletedEvent : DomainEvent, INotification;
