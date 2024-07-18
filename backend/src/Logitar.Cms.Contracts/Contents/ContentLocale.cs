using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Languages;

namespace Logitar.Cms.Contracts.Contents;

public class ContentLocale
{
  public Guid Id { get; set; }

  public ContentItem Item { get; set; }
  public Language? Language { get; set; }

  public string UniqueName { get; set; }

  public List<FieldValue> Fields { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public ContentLocale()
  {
    Item = new ContentItem();

    UniqueName = string.Empty;

    Fields = [];
  }

  public ContentLocale(ContentItem item, string uniqueName)
  {
    Item = item;

    UniqueName = uniqueName;

    Fields = [];
  }

  public override bool Equals(object obj) => obj is ContentLocale locale && locale.Item.Id == Item.Id && locale.Language?.Id == Language?.Id;
  public override int GetHashCode() => HashCode.Combine(Item.Id, Language?.Id);
  public override string ToString() => $"{UniqueName} | {GetType()} (ItemId={Item.Id}, LanguageId={Language?.Id.ToString() ?? "<null>"})";
}
