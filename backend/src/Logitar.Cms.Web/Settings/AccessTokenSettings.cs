namespace Logitar.Cms.Web.Settings;

public record AccessTokenSettings
{
  public int Lifetime { get; set; } = 300;
  public string TokenType { get; set; } = "at+jwt";
}
