using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public class ContentDeletedEvent : DomainEvent, INotification;
