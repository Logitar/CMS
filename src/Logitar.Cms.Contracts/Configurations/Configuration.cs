namespace Logitar.Cms.Contracts.Configurations;

public record Configuration : Aggregate
{
  public string Secret { get; set; } = string.Empty;

  public LoggingSettings LoggingSettings { get; set; } = new();

  public UsernameSettings UsernameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
}
