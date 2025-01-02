namespace Logitar.Cms.Core.Fields.Models;

public record UpdateFieldTypePayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public BooleanSettingsModel? Boolean { get; set; }
  public DateTimeSettingsModel? DateTime { get; set; }
  public NumberSettingsModel? Number { get; set; }
  public RichTextSettingsModel? RichText { get; set; }
  public StringSettingsModel? String { get; set; }
}
