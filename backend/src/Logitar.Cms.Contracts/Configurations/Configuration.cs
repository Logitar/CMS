namespace Logitar.Cms.Contracts.Configurations;

public class Configuration : Aggregate
{
  public string Secret { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; }
  public PasswordSettings PasswordSettings { get; set; }
  public bool RequireUniqueName { get; set; }

  public LoggingSettings LoggingSettings { get; set; }

  public Configuration() : this(string.Empty)
  {
  }

  public Configuration(string secret)
  {
    Secret = secret;

    UniqueNameSettings = new();
    PasswordSettings = new();

    LoggingSettings = new();
  }
}
