namespace Logitar.Cms.Web.Settings;

public record CorsSettings
{
  public string[]? AllowedOrigins { get; set; }
  public string[]? AllowedMethods { get; set; }
  public string[]? AllowedHeaders { get; set; }
}
