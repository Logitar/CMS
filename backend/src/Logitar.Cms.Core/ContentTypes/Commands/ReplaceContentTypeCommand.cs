using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record ReplaceContentTypeCommand(Guid Id, ReplaceContentTypePayload Payload, long? Version) : Activity, IRequest<ContentsType?>;
