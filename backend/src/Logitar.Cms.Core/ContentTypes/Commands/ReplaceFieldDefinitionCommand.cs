using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record ReplaceFieldDefinitionCommand(Guid ContentTypeId, Guid FieldId, ReplaceFieldDefinitionPayload Payload, long? Version) : Activity, IRequest<ContentsType?>;
