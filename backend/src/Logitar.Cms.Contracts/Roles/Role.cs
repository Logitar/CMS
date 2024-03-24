namespace Logitar.Cms.Contracts.Roles;

public class Role : Aggregate
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Role() : this(string.Empty)
  {
  }

  public Role(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
