namespace Logitar.Cms.Web.Constants;

public static class Schemes
{
  public const string Basic = "Basic";
  public const string Session = "Session";

  public static string[] GetEnabled(IConfiguration configuration)
  {
    List<string> schemes = new(capacity: 2);

    if (configuration.GetValue<bool>("EnableBasicAuthentication"))
    {
      schemes.Add(Basic);
    }

    return [.. schemes];
  }
}
