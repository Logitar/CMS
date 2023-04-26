using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Users.Events;

public record UserSignedIn : DomainEvent, INotification;
