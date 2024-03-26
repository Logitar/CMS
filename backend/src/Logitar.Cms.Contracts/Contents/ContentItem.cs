using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Contracts.Contents;

public class ContentItem : Aggregate
{
  public ContentType ContentType { get; set; }

  public ContentLocale Invariant { get; set; }
  public List<ContentLocale> Locales { get; set; }

  public ContentItem()
  {
    ContentType = new ContentType();

    Invariant = new ContentLocale(this, uniqueName: string.Empty);
    Locales = [];
  }

  public ContentItem(ContentType contentType, ContentLocale invariant) : this()
  {
    ContentType = contentType;
    Invariant = invariant;
  }

  public override string ToString() => $"{Invariant.DisplayName ?? Invariant.UniqueName} | {base.ToString()}";
}
