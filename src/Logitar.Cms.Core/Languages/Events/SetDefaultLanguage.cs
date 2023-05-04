using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages.Events;

public record SetDefaultLanguage : DomainEvent, INotification
{
  public bool IsDefault { get; init; }
}
