using Logitar.Cms.Core.Security;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Events;

public record SessionCreated : DomainEvent, INotification
{
  public Pbkdf2? Secret { get; init; }

  public string? IpAddress { get; init; }
  public string? AdditionalInformation { get; init; }
}
