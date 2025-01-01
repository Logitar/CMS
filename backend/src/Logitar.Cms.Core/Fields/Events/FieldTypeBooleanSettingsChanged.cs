using Logitar.Cms.Core.Fields.Settings;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Fields.Events;

public record FieldTypeBooleanSettingsChanged(BooleanSettings Settings) : DomainEvent, INotification;
