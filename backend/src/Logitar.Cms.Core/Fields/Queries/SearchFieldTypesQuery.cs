using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Fields.Queries;

public record SearchFieldTypesQuery(SearchFieldTypesPayload Payload) : IRequest<SearchResults<FieldType>>;
