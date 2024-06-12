using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Localization;

namespace Logitar.Cms.Contracts.Contents;

public record ContentLocale
{
  public string UniqueName { get; set; }

  public ContentItem Item { get; set; }
  public Language? Language { get; set; }

  public Actor CreatedBy { get; set; } = Actor.System;
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = Actor.System;
  public DateTime UpdatedOn { get; set; }

  public ContentLocale()
  {
    UniqueName = string.Empty;

    Item = new ContentItem(new ContentsType(), this);
  }

  public ContentLocale(string uniqueName, ContentItem item)
  {
    UniqueName = uniqueName;

    Item = item;
  }

  public override string ToString() => $"{UniqueName} | {base.ToString()} (ItemId={Item.Id}, LanguageId={Language?.Id.ToString() ?? "<null>"})";
}
