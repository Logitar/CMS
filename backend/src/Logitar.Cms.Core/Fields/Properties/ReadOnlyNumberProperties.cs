using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public record ReadOnlyNumberProperties : FieldTypeProperties, INumberProperties
{
  public override DataType DataType => DataType.Number;

  public double? MinimumValue { get; }
  public double? MaximumValue { get; }
  public double? Step { get; }

  public ReadOnlyNumberProperties() : this(null, null, null)
  {
  }

  public ReadOnlyNumberProperties(INumberProperties number) : this(number.MinimumValue, number.MaximumValue, number.Step)
  {
  }

  [JsonConstructor]
  public ReadOnlyNumberProperties(double? minimumValue, double? maximumValue, double? step)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    Step = step;
    new NumberPropertiesValidator().ValidateAndThrow(this);
  }
}
