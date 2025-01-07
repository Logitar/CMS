using Logitar.Cms.Core.Fields.Models;

namespace Logitar.Cms.Core.Fields.Settings;

internal static class SettingsExtensions
{
  public static SelectSettings ToSelectSettings(this SelectSettingsModel select)
  {
    IReadOnlyCollection<SelectOption> options = select.Options.Select(option => new SelectOption(option)).ToArray();
    return new SelectSettings(select.IsMultiple, options);
  }
}
