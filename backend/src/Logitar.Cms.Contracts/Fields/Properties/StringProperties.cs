namespace Logitar.Cms.Contracts.Fields.Properties;

public record StringProperties : IStringProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }
}
