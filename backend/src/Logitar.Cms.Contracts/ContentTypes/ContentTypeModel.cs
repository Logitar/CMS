namespace Logitar.Cms.Contracts.ContentTypes;

public class ContentTypeModel : AggregateModel
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ContentTypeModel() : this(string.Empty)
  {
  }

  public ContentTypeModel(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} (Id={Id})";
}
