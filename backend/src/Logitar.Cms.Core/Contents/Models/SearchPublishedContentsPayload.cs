using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Contents.Models;

public record SearchPublishedContentsPayload
{
  public ContentFilter Content { get; set; } = new();
  public ContentTypeFilter ContentType { get; set; } = new();
  public LanguageFilter Language { get; set; } = new();
  public TextSearch Search { get; set; } = new();

  public List<PublishedContentSortOption> Sort { get; set; } = [];

  public int Skip { get; set; }
  public int Limit { get; set; }
}
