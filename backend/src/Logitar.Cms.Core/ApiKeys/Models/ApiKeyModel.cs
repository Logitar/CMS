using Logitar.Cms.Core.Roles.Models;

namespace Logitar.Cms.Core.ApiKeys.Models;

public class ApiKeyModel : AggregateModel
{
  public string? XApiKey { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public List<RoleModel> Roles { get; set; } = [];

  public ApiKeyModel()
  {
  }

  public ApiKeyModel(string displayName)
  {
    DisplayName = displayName;
  }

  public override string ToString() => $"{DisplayName} | {base.ToString()}";
}
