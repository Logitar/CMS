namespace Logitar.Cms.Web.Constants;

public static class Schemes
{
  public const string Session = nameof(Session);

  public static string[] GetEnabled(IConfiguration configuration)
  {
    List<string> schemes = new(capacity: 4)
    {
      // TODO(fpion): X-API-Key
      // TODO(fpion): Bearer
      Session
    };

    //if (configuration.GetValue<bool>("EnableBasicAuthentication"))
    //{
    //  schemes.Add(Basic);
    //} // TODO(fpion): Basic

    return [.. schemes];
  }
}
