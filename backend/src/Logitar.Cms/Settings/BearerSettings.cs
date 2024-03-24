namespace Logitar.Cms.Settings;

internal record BearerSettings
{
  public int LifetimeSeconds { get; set; }
  public string TokenType { get; set; }

  public BearerSettings() : this(string.Empty)
  {
  }

  public BearerSettings(string tokenType)
  {
    TokenType = tokenType;
  }
}
