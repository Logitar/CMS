using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

public record ReadContentTypeQuery(Guid? Id, string? UniqueName) : Activity, IRequest<CmsContentType?>;
