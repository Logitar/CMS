using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public class LanguageSetDefaultEvent : DomainEvent, INotification
{
  public bool IsDefault { get; }

  public LanguageSetDefaultEvent(bool isDefault)
  {
    IsDefault = isDefault;
  }
}
