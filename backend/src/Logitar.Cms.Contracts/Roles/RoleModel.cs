namespace Logitar.Cms.Contracts.Roles;

public class RoleModel : AggregateModel
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public RoleModel() : this(string.Empty)
  {
  }

  public RoleModel(string uniqueName)
  {
    UniqueName = uniqueName;

    CustomAttributes = [];
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
