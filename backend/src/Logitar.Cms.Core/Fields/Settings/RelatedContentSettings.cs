using Logitar.Cms.Core.Contents;

namespace Logitar.Cms.Core.Fields.Settings;

public record RelatedContentSettings : FieldTypeSettings, IRelatedContentSettings
{
  public override DataType DataType { get; } = DataType.RelatedContent;

  public ContentTypeId ContentTypeId { get; }
  public bool IsMultiple { get; }

  [JsonConstructor]
  public RelatedContentSettings(ContentTypeId contentTypeId, bool isMultiple = false)
  {
    ContentTypeId = contentTypeId;
    IsMultiple = isMultiple;
  }

  public RelatedContentSettings(IRelatedContentSettings relatedContent)
  {
    ContentTypeId = relatedContent.ContentTypeId;
    IsMultiple = relatedContent.IsMultiple;
  }
}
