using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.Contents;

public record SearchContentsPayload : SearchPayload
{
  public Guid? ContentTypeId { get; set; }
  public Guid? LanguageId { get; set; }

  public new List<ContentSortOption>? Sort { get; set; }
}
