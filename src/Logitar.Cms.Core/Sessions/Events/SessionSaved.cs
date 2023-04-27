using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Sessions.Events;

public abstract record SessionSaved : DomainEvent
{
  public string? IpAddress { get; init; }
  public string? AdditionalInformation { get; init; }
}
