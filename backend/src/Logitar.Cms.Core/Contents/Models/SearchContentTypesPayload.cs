using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Contents.Models;

public record SearchContentTypesPayload : SearchPayload
{
  public bool? IsInvariant { get; set; }

  public new List<ContentTypeSortOption> Sort { get; set; } = [];
}
