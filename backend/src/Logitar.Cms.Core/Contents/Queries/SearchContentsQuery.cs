using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

public record SearchContentsQuery(SearchContentsPayload Payload) : IRequest<SearchResults<ContentLocale>>;
