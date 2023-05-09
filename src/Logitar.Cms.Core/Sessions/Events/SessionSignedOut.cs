using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Events;

public record SessionSignedOut : DomainEvent, INotification;
