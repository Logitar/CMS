namespace Logitar.Cms.Core.Contents.Models;

public class ContentTypeModel : AggregateModel
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
}
