using Logitar.Cms.Core.ContentTypes;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Contents.Events;

public record ContentCreatedEvent(ContentTypeId ContentTypeId, UniqueNameUnit UniqueName) : DomainEvent, INotification;
