namespace Logitar.Cms.Contracts.ContentTypes;

public class ContentsType : Aggregate
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldDefinition> Fields { get; set; }

  public ContentsType() : this(string.Empty)
  {
  }

  public ContentsType(string uniqueName)
  {
    UniqueName = uniqueName;
    Fields = [];
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
