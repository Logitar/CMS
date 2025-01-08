namespace Logitar.Cms.Core.Fields.Models;

public class FieldTypeModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public DataType DataType { get; set; }
  public BooleanSettingsModel? Boolean { get; set; }
  public DateTimeSettingsModel? DateTime { get; set; }
  public NumberSettingsModel? Number { get; set; }
  public RelatedContentSettingsModel? RelatedContent { get; set; }
  public RichTextSettingsModel? RichText { get; set; }
  public SelectSettingsModel? Select { get; set; }
  public StringSettingsModel? String { get; set; }
  public TagsSettingsModel? Tags { get; set; }
}
