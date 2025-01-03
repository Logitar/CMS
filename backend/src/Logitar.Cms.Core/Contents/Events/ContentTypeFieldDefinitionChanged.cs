using Logitar.Cms.Core.Fields;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentTypeFieldDefinitionChanged(Guid FieldId, FieldDefinition FieldDefinition) : DomainEvent, INotification;
