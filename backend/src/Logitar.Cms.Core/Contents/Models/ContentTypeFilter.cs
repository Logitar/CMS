namespace Logitar.Cms.Core.Contents.Models;

public record ContentTypeFilter
{
  public List<int> Ids { get; set; } = [];
  public List<Guid> Uids { get; set; } = [];
  public List<string> Names { get; set; } = [];
}
