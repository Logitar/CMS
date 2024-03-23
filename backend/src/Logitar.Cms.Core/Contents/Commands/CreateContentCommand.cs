using Logitar.Cms.Contracts.Contents;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record CreateContentCommand(CreateContentPayload Payload) : Request, IRequest<ContentItem>;
