using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record CreateFieldDefinitionCommand(CreateFieldDefinitionPayload Payload) : Activity, IRequest<ContentsType>;
