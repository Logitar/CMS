using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public record LanguageCreatedEvent(bool IsDefault, LocaleUnit Locale) : DomainEvent, INotification;
