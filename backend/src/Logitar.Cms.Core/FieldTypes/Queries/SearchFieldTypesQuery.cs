using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Queries;

public record SearchFieldTypesQuery(SearchFieldTypesPayload Payload) : IRequest<SearchResults<FieldType>>;
