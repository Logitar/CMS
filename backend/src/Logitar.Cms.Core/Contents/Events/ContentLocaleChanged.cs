using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentLocaleChanged(LanguageId? LanguageId, ContentLocale Locale) : DomainEvent, INotification;
