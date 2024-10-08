namespace Logitar.Cms.Contracts.ApiKeys;

public class ApiKeyModel : AggregateModel
{
  public string? XApiKey { get; set; }

  public string DisplayName { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public ApiKeyModel() : this(string.Empty)
  {
  }

  public ApiKeyModel(string displayName)
  {
    DisplayName = displayName;

    CustomAttributes = [];
  }

  public override string ToString() => $"{DisplayName} | {base.ToString()}";
}
