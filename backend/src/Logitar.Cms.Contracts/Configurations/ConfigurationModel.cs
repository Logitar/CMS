namespace Logitar.Cms.Contracts.Configurations;

public class ConfigurationModel : AggregateModel
{
  public string Secret { get; set; }

  public UniqueNameSettingsModel UniqueNameSettings { get; set; }
  public PasswordSettingsModel PasswordSettings { get; set; }
  public bool RequireUniqueEmail { get; set; }

  public LoggingSettingsModel LoggingSettings { get; set; }

  public ConfigurationModel() : this(string.Empty)
  {
  }

  public ConfigurationModel(string secret)
  {
    Secret = secret;

    UniqueNameSettings = new();
    PasswordSettings = new();
    LoggingSettings = new();
  }
}
