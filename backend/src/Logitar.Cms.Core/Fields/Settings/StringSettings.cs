using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core.Fields.Settings;

public record StringSettings : FieldTypeSettings, IStringSettings
{
  public override DataType DataType { get; } = DataType.String;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }
  public string? Pattern { get; }

  public StringSettings(int? minimumLength = null, int? maximumLength = null, string? pattern = null)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
    new StringSettingsValidator().ValidateAndThrow(this);
  }

  public StringSettings(IStringSettings @string)
  {
    MinimumLength = @string.MinimumLength;
    MaximumLength = @string.MaximumLength;
    Pattern = @string.Pattern;
    new StringSettingsValidator().ValidateAndThrow(this);
  }
}
