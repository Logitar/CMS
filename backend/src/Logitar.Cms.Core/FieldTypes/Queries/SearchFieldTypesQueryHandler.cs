using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Queries;

internal class SearchFieldTypesQueryHandler : IRequestHandler<SearchFieldTypesQuery, SearchResults<FieldType>>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;

  public SearchFieldTypesQueryHandler(IFieldTypeQuerier fieldTypeQuerier)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
  }

  public async Task<SearchResults<FieldType>> Handle(SearchFieldTypesQuery query, CancellationToken cancellationToken)
  {
    return await _fieldTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
