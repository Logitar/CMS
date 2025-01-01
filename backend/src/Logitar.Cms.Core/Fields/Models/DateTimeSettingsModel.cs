using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record DateTimeSettingsModel : IDateTimeSettings
{
  public DateTime? MinimumValue { get; }
  public DateTime? MaximumValue { get; }

  public DateTimeSettingsModel()
  {
  }

  public DateTimeSettingsModel(IDateTimeSettings dateTime)
  {
    MinimumValue = dateTime.MinimumValue;
    MaximumValue = dateTime.MaximumValue;
  }
}
