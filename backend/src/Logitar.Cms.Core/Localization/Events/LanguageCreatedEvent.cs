using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public class LanguageCreatedEvent : DomainEvent, INotification
{
  public bool IsDefault { get; }
  public LocaleUnit Locale { get; }

  public LanguageCreatedEvent(bool isDefault, LocaleUnit locale)
  {
    IsDefault = isDefault;
    Locale = locale;
  }
}
