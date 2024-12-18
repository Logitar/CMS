using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Localization.Events;

public record LanguageSetDefault(bool IsDefault) : DomainEvent, INotification;
