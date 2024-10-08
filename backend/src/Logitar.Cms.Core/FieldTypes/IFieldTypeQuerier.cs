using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Core.FieldTypes;

public interface IFieldTypeQuerier
{
  Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken = default);
}
