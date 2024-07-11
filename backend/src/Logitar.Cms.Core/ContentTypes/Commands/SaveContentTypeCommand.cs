using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record SaveContentTypeCommand(ContentTypeAggregate ContentType) : IRequest;
