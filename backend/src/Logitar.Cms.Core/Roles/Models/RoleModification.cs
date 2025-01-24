namespace Logitar.Cms.Core.Roles.Models;

public record RoleModification
{
  public string Role { get; set; } = string.Empty;
  public CollectionAction Action { get; set; }

  public RoleModification()
  {
  }

  public RoleModification(string role, CollectionAction action = CollectionAction.Add)
  {
    Role = role;
    Action = action;
  }
}
