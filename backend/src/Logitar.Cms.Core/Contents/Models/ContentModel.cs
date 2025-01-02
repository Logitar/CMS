namespace Logitar.Cms.Core.Contents.Models;

public class ContentModel : AggregateModel
{
  public ContentTypeModel ContentType { get; set; } = new();

  public ContentLocaleModel Invariant { get; set; } = new();
  public List<ContentLocaleModel> Locales { get; set; } = [];

  public override string ToString() => $"{Invariant} | {base.ToString()}";
}
