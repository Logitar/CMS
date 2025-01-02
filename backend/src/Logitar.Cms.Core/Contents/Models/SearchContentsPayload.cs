using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Contents.Models;

public record SearchContentsPayload : SearchPayload
{
  public Guid? ContentTypeId { get; set; }
  public Guid? LanguageId { get; set; }

  public new List<ContentSortOption> Sort { get; set; } = [];
}
