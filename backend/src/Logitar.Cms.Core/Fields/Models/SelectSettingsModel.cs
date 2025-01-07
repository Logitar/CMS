using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record SelectSettingsModel
{
  public bool IsMultiple { get; set; }
  public List<SelectOptionModel> Options { get; set; } = [];

  public SelectSettingsModel()
  {
  }

  public SelectSettingsModel(ISelectSettings select)
  {
    IsMultiple = select.IsMultiple;

    foreach (SelectOption option in select.Options)
    {
      Options.Add(new SelectOptionModel(option));
    }
  }
}
