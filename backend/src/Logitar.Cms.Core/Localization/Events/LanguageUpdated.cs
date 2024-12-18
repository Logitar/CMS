using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public record LanguageUpdated : DomainEvent, INotification
{
  public Locale? Locale { get; set; }

  [JsonIgnore]
  public bool HasChanges => Locale != null;
}
