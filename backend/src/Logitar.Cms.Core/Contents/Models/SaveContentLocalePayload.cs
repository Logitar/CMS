namespace Logitar.Cms.Core.Contents.Models;

public record SaveContentLocalePayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
}
