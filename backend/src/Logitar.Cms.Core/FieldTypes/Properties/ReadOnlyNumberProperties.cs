using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

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
