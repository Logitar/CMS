using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public record LanguageCreated(Locale Locale, bool IsDefault) : DomainEvent, INotification;
