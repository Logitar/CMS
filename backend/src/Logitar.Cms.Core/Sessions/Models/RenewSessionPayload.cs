namespace Logitar.Cms.Core.Sessions.Models;

public record RenewSessionPayload
{
  public string RefreshToken { get; set; } = string.Empty;

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public RenewSessionPayload()
  {
  }

  public RenewSessionPayload(string refreshToken, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    RefreshToken = refreshToken;

    if (customAttributes != null)
    {
      CustomAttributes.AddRange(customAttributes);
    }
  }
}
