namespace Logitar.Cms.Contracts.Configurations;

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }

  public LoggingSettings() : this(LoggingExtent.ActivityOnly, onlyErrors: false)
  {
  }

  public LoggingSettings(ILoggingSettings settings) : this(settings.Extent, settings.OnlyErrors)
  {
  }

  public LoggingSettings(LoggingExtent extent, bool onlyErrors)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
  }
}
