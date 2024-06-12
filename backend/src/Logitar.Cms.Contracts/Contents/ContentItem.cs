using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Contracts.Contents;

public class ContentItem : Aggregate
{
  public ContentsType ContentType { get; set; }

  public ContentLocale Invariant { get; set; }
  public List<ContentLocale> Locales { get; set; }

  public ContentItem()
  {
    ContentType = new ContentsType();

    Invariant = new ContentLocale(string.Empty, this);
    Locales = [];
  }

  public ContentItem(ContentsType contentType, ContentLocale invariant) : this()
  {
    ContentType = contentType;

    Invariant = invariant;
  }

  public override string ToString() => $"{Invariant.UniqueName} | {base.ToString()}";
}
