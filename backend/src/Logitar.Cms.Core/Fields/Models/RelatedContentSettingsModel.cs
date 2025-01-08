using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Models;

public record RelatedContentSettingsModel
{
  public Guid ContentTypeId { get; set; }
  public bool IsMultiple { get; set; }

  public RelatedContentSettingsModel()
  {
  }

  public RelatedContentSettingsModel(IRelatedContentSettings relatedContent)
  {
    ContentTypeId = relatedContent.ContentTypeId.ToGuid();
    IsMultiple = relatedContent.IsMultiple;
  }
}
