using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Events;

public class LanguageDeletedEvent : DomainEvent, INotification;
