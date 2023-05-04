using Logitar.Cms.Core.Security;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Users.Events;

public record PasswordChanged(Pbkdf2 Password) : DomainEvent, INotification;
