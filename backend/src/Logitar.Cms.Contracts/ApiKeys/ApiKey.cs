using Logitar.Cms.Contracts.Roles;

namespace Logitar.Cms.Contracts.ApiKeys;

public class ApiKey : Aggregate
{
  public string? XApiKey { get; set; }

  public string DisplayName { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public List<Role> Roles { get; set; }

  public ApiKey() : this(string.Empty)
  {
  }

  public ApiKey(string displayName)
  {
    DisplayName = displayName;
    CustomAttributes = [];
    Roles = [];
  }

  public override string ToString() => $"{DisplayName} | {base.ToString()}";
}
