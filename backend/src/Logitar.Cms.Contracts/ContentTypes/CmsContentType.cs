namespace Logitar.Cms.Contracts.ContentTypes;

public class CmsContentType : Aggregate
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldDefinition> Fields { get; set; }

  public CmsContentType() : this(string.Empty)
  {
  }

  public CmsContentType(string uniqueName)
  {
    UniqueName = uniqueName;

    Fields = [];
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
