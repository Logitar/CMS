using Logitar.Cms.Core.Fields.Settings;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeStringSettingsChanged(StringSettings Settings) : DomainEvent, INotification;
