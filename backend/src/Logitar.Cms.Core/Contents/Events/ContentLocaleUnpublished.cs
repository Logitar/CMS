using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentLocaleUnpublished(LanguageId? LanguageId) : DomainEvent, INotification;
