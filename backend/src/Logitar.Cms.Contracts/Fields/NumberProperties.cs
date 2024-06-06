namespace Logitar.Cms.Contracts.Fields;

public record NumberProperties
{
  public double? MinimumValue { get; set; }
  public double? MaximumValue { get; set; }
  public double? Step { get; set; }
}
