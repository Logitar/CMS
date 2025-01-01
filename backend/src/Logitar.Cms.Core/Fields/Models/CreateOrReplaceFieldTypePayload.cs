namespace Logitar.Cms.Core.Fields.Models;

public record CreateOrReplaceFieldTypePayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public BooleanSettingsModel? Boolean { get; set; }
  public DateTimeSettingsModel? DateTime { get; set; }
  public NumberSettingsModel? Number { get; set; }
  public RichTextSettingsModel? RichText { get; set; }
  public StringSettingsModel? String { get; set; }
}
