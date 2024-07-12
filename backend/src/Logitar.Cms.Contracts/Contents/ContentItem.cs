using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Contracts.Contents;

public class ContentItem : Aggregate
{
  public CmsContentType ContentType { get; set; }

  public ContentLocale Invariant { get; set; }
  public List<ContentLocale> Locales { get; set; }

  public ContentItem() : this(new CmsContentType(), new ContentLocale())
  {
  }

  public ContentItem(CmsContentType contentType, ContentLocale invariant)
  {
    ContentType = contentType;

    Invariant = invariant;
    Locales = [];
  }

  public override string ToString() => $"{Invariant.UniqueName} | {base.ToString()}";
}
