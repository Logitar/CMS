using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record DateTimeSettingsModel : IDateTimeSettings
{
  public DateTime? MinimumValue { get; set; }
  public DateTime? MaximumValue { get; set; }

  public DateTimeSettingsModel()
  {
  }

  public DateTimeSettingsModel(IDateTimeSettings dateTime)
  {
    MinimumValue = dateTime.MinimumValue;
    MaximumValue = dateTime.MaximumValue;
  }
}
