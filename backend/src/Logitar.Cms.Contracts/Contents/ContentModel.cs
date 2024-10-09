using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Contracts.Contents;

public class ContentModel : AggregateModel
{
  public ContentTypeModel ContentType { get; set; }

  public ContentLocaleModel Invariant { get; set; }
  public List<ContentLocaleModel> Locales { get; set; }

  public ContentModel()
  {
    ContentType = new();

    Invariant = new(this, string.Empty);
    Locales = [];
  }

  public override string ToString() => $"{Invariant.UniqueName} | {base.ToString()}";
}
