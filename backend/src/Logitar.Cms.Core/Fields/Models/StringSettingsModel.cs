using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record StringSettingsModel : IStringSettings
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }

  public StringSettingsModel()
  {
  }

  public StringSettingsModel(IStringSettings @string)
  {
    MinimumLength = @string.MinimumLength;
    MaximumLength = @string.MaximumLength;
    Pattern = @string.Pattern;
  }
}
