using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public record LanguageCreated(bool IsDefault, Locale Locale) : DomainEvent, INotification;
