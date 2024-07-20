using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.FieldTypes;

public interface IFieldTypeQuerier
{
  Task<FieldType> ReadAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken = default);
  Task<FieldType?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldType?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FieldType?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FieldType>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken = default);
}
