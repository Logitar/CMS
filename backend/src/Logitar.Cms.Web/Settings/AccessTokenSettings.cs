namespace Logitar.Cms.Web.Settings;

public record AccessTokenSettings
{
  public int LifetimeSeconds { get; set; }
  public string Type { get; set; }

  public AccessTokenSettings() : this(string.Empty)
  {
  }

  public AccessTokenSettings(string type)
  {
    Type = type;
  }
}
