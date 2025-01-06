namespace Logitar.Cms.Web.Constants;

public static class Schemes
{
  public const string Basic = "Basic";
  public const string Bearer = "Bearer";
  public const string Session = "Session";

  public static string[] GetEnabled(IConfiguration configuration)
  {
    List<string> schemes = new(capacity: 3)
    {
      Bearer,
      Session
    };

    if (configuration.GetValue<bool>("EnableBasicAuthentication"))
    {
      schemes.Add(Basic);
    }

    return [.. schemes];
  }
}
