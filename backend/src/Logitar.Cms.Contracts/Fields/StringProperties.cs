namespace Logitar.Cms.Contracts.Fields;

public record StringProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }
}
