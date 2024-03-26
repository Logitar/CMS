using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Localization;

namespace Logitar.Cms.Contracts.Contents;

public record ContentLocale
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ContentItem Item { get; set; }
  public Language? Language { get; set; }

  public Actor CreatedBy { get; set; } = Actor.System;
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = Actor.System;
  public DateTime UpdatedOn { get; set; }

  public ContentLocale()
  {
    UniqueName = string.Empty;

    Item = new ContentItem(new ContentType(), this);
  }

  public ContentLocale(ContentItem item, string uniqueName)
  {
    Item = item;
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()} (ItemId={Item.Id}, LanguageId={Language?.Id.ToString() ?? "<null>"})";
}
