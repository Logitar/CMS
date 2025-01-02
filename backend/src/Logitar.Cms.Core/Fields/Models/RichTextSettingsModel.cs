using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record RichTextSettingsModel : IRichTextSettings
{
  public string ContentType { get; set; } = string.Empty;
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }

  public RichTextSettingsModel()
  {
  }

  public RichTextSettingsModel(IRichTextSettings richText)
  {
    ContentType = richText.ContentType;
    MinimumLength = richText.MinimumLength;
    MaximumLength = richText.MaximumLength;
  }
}
