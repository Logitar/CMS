namespace Logitar.Cms.Core.Contents.Models;

public record ContentFilter
{
  public List<int> Ids { get; set; } = [];
  public List<Guid> Uids { get; set; } = [];
}
