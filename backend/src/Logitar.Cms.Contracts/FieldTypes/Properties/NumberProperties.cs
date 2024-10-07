namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record NumberProperties : INumberProperties
{
  public double? MinimumValue { get; set; }
  public double? MaximumValue { get; set; }
  public double? Step { get; set; }

  public NumberProperties() : this(minimumValue: null, maximumValue: null, step: null)
  {
  }

  public NumberProperties(INumberProperties number) : this(number.MinimumValue, number.MaximumValue, number.Step)
  {
  }

  public NumberProperties(double? minimumValue, double? maximumValue, double? step)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    Step = step;
  }
}
