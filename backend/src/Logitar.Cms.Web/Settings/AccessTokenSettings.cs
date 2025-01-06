namespace Logitar.Cms.Web.Settings;

public record AccessTokenSettings
{
  public string TokenType { get; set; } = "at+jwt";
  public int LifetimeSeconds { get; set; } = 300;
  public string Secret { get; set; } = string.Empty;
}
