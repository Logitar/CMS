namespace Logitar.Cms.Contracts.Contents;

public record ContentLocale
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ContentItem Item { get; set; }

  public ContentLocale() : this(new ContentItem(), string.Empty)
  {
  }

  public ContentLocale(ContentItem item, string uniqueName)
  {
    Item = item;
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()} (ItemId={Item.Id})"; // TODO(fpion): LanguageId
}
