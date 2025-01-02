using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core.Fields.Settings;

public record DateTimeSettings : FieldTypeSettings, IDateTimeSettings
{
  public override DataType DataType { get; } = DataType.DateTime;

  public DateTime? MinimumValue { get; }
  public DateTime? MaximumValue { get; }

  [JsonConstructor]
  public DateTimeSettings(DateTime? minimumValue = null, DateTime? maximumValue = null)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    new DateTimeSettingsValidator().ValidateAndThrow(this);
  }

  public DateTimeSettings(IDateTimeSettings dateTime)
  {
    MinimumValue = dateTime.MinimumValue;
    MaximumValue = dateTime.MaximumValue;
    new DateTimeSettingsValidator().ValidateAndThrow(this);
  }
}
