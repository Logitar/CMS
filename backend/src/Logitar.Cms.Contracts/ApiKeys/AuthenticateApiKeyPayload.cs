namespace Logitar.Cms.Contracts.ApiKeys;

public record AuthenticateApiKeyPayload
{
  public string XApiKey { get; set; }

  public AuthenticateApiKeyPayload() : this(string.Empty)
  {
  }

  public AuthenticateApiKeyPayload(string xApiKey)
  {
    XApiKey = xApiKey;
  }
}
