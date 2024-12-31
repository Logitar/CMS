namespace Logitar.Cms.Settings;

public record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
