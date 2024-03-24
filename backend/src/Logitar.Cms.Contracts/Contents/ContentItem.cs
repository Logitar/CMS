using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Contracts.Contents;

public class ContentItem : Aggregate
{
  public ContentType ContentType { get; set; }

  public List<ContentLocale> Locales { get; set; } = [];

  public ContentItem() : this(new ContentType())
  {
  }

  public ContentItem(ContentType contentType)
  {
    ContentType = contentType;
  }

  //public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}"; // TODO(fpion): invariant
}
