namespace Logitar.Cms.Contracts.Fields.Properties;

public record NumberProperties : INumberProperties
{
  public double? MinimumValue { get; set; }
  public double? MaximumValue { get; set; }
  public double? Step { get; set; }
}
