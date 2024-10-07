namespace Logitar.Cms.Contracts.ApiKeys;

public record CreateApiKeyPayload // ISSUE: https://github.com/Logitar/CMS/issues/11
{
  public string DisplayName { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public CreateApiKeyPayload() : this(string.Empty)
  {
  }

  public CreateApiKeyPayload(string displayName)
  {
    DisplayName = displayName;

    CustomAttributes = [];
  }
}
