using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record CreateFieldDefinitionCommand(Guid ContentTypeId, CreateFieldDefinitionPayload Payload) : Activity, IRequest<CmsContentType>;
