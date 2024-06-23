using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record UpdateContentTypeCommand(Guid Id, UpdateContentTypePayload Payload) : Activity, IRequest<ContentsType?>;
