using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentLocalePublished(LanguageId? LanguageId) : DomainEvent, INotification;
