using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core.Fields.Settings;

public record BooleanSettings : FieldTypeSettings, IBooleanSettings
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.Boolean;

  public BooleanSettings()
  {
    new BooleanSettingsValidator().ValidateAndThrow(this);
  }

  public BooleanSettings(IBooleanSettings _)
  {
    new BooleanSettingsValidator().ValidateAndThrow(this);
  }
}
