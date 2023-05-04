using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Users.Events;

public record EmailChanged : DomainEvent, INotification
{
  public ReadOnlyEmail? Email { get; init; }
  public VerificationAction? VerificationAction { get; init; }
}
