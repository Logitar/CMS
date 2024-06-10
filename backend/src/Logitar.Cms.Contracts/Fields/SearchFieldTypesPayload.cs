using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.Fields;

public record SearchFieldTypesPayload : SearchPayload
{
  public DataType? DataType { get; set; }

  public new List<FieldTypeSortOption>? Sort { get; set; }
}
