namespace Logitar.Cms.Web.Settings;

public record OpenAuthenticationSettings
{
  public AccessTokenSettings AccessToken { get; set; } = new();
}
