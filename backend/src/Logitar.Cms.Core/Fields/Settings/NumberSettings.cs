using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core.Fields.Settings;

public record NumberSettings : FieldTypeSettings, INumberSettings
{
  public override DataType DataType { get; } = DataType.Number;

  public double? MinimumValue { get; }
  public double? MaximumValue { get; }
  public double? Step { get; }

  [JsonConstructor]
  public NumberSettings(double? minimumValue = null, double? maximumValue = null, double? step = null)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    Step = step;
    new NumberSettingsValidator().ValidateAndThrow(this);
  }

  public NumberSettings(INumberSettings number)
  {
    MinimumValue = number.MinimumValue;
    MaximumValue = number.MaximumValue;
    Step = number.Step;
    new NumberSettingsValidator().ValidateAndThrow(this);
  }
}
