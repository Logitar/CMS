using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public record LanguageLocaleChanged(Locale Locale) : DomainEvent, INotification;
