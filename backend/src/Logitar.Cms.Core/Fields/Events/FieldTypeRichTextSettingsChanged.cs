using Logitar.Cms.Core.Fields.Settings;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeRichTextSettingsChanged(RichTextSettings Settings) : DomainEvent, INotification;
