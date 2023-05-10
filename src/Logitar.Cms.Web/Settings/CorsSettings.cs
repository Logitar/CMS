namespace Logitar.Cms.Web.Settings;

public record CorsSettings
{
  public bool AllowCredentials { get; set; }
  public string[]? AllowedOrigins { get; set; }
  public string[]? AllowedMethods { get; set; }
  public string[]? AllowedHeaders { get; set; }
}
