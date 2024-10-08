namespace Logitar.Cms.Contracts.Configurations;

public record LoggingSettingsModel : ILoggingSettings
{
  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }

  public LoggingSettingsModel() : this(LoggingExtent.None, onlyErrors: false)
  {
  }

  public LoggingSettingsModel(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  public LoggingSettingsModel(LoggingExtent extent, bool onlyErrors)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
  }
}
