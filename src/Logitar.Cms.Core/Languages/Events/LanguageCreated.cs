using Logitar.EventSourcing;
using MediatR;
using System.Globalization;

namespace Logitar.Cms.Core.Languages.Events;

public record LanguageCreated(CultureInfo Locale) : DomainEvent, INotification;
