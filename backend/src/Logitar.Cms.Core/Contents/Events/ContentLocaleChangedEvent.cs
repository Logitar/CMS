using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentLocaleChangedEvent(LanguageId? LanguageId, ContentLocaleUnit Locale) : DomainEvent, INotification;
