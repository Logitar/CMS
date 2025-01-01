using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record NumberSettingsModel : INumberSettings
{
  public double? MinimumValue { get; set; }
  public double? MaximumValue { get; set; }
  public double? Step { get; set; }

  public NumberSettingsModel()
  {
  }

  public NumberSettingsModel(INumberSettings number)
  {
    MinimumValue = number.MinimumValue;
    MaximumValue = number.MaximumValue;
    Step = number.Step;
  }
}
