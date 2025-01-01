using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Search;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Fields;

public interface IFieldTypeQuerier
{
  Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken = default);
}
