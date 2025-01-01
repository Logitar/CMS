using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Fields.Models;

public record SearchFieldTypesPayload : SearchPayload
{
  public DataType? DataType { get; set; }

  public new List<FieldTypeSortOption> Sort { get; set; } = [];
}
