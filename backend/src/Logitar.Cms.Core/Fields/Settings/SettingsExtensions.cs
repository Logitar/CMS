using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields.Models;

namespace Logitar.Cms.Core.Fields.Settings;

internal static class SettingsExtensions
{
  public static RelatedContentSettings ToRelatedContentSettings(this RelatedContentSettingsModel relatedContent)
  {
    ContentTypeId contentTypeId = new(relatedContent.ContentTypeId);
    return new RelatedContentSettings(contentTypeId, relatedContent.IsMultiple);
  }

  public static SelectSettings ToSelectSettings(this SelectSettingsModel select)
  {
    IReadOnlyCollection<SelectOption> options = select.Options.Select(option => new SelectOption(option)).ToArray();
    return new SelectSettings(select.IsMultiple, options);
  }
}
