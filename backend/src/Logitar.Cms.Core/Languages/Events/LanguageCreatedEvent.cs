using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Languages.Events;

public class LanguageCreatedEvent : DomainEvent, INotification
{
  public bool IsDefault { get; private set; }
  public LocaleUnit Locale { get; private set; }

  public LanguageCreatedEvent(bool isDefault, LocaleUnit locale)
  {
    IsDefault = isDefault;
    Locale = locale;
  }
}
