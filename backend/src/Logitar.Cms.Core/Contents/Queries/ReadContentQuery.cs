using Logitar.Cms.Contracts.Contents;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

public record ReadContentQuery(Guid Id) : Activity, IRequest<ContentItem?>;
