using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public class ContentLocaleChangedEvent : DomainEvent, INotification
{
  public LanguageId? LanguageId { get; }
  public ContentLocaleUnit Locale { get; }

  public ContentLocaleChangedEvent(LanguageId? languageId, ContentLocaleUnit locale)
  {
    LanguageId = languageId;
    Locale = locale;
  }
}
