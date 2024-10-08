using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.FieldTypes;

public record SearchFieldTypesPayload : SearchPayload
{
  public DataType? DataType { get; set; }

  public new List<FieldTypeSortOption> Sort { get; set; } = [];
}
