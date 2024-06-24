using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record UpdateFieldDefinitionCommand(Guid ContentTypeId, Guid FieldId, UpdateFieldDefinitionPayload Payload) : Activity, IRequest<ContentsType?>;
