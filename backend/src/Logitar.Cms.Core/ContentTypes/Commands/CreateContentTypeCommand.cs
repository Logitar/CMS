using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record CreateContentTypeCommand(CreateContentTypePayload Payload) : Activity, IRequest<CmsContentType>;
