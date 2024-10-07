using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.ContentTypes;

public record SearchContentTypesPayload : SearchPayload
{
  public bool? IsInvariant { get; set; }

  public new List<ContentTypeSortOption>? Sort { get; set; }
}
