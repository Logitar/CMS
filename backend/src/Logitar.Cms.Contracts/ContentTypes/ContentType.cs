namespace Logitar.Cms.Contracts.ContentTypes;

public class ContentType : Aggregate
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ContentType() : this(string.Empty)
  {
  }

  public ContentType(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
