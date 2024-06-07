using FluentValidation;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Configurations.Validators;

namespace Logitar.Cms.Core.Configurations;

public record ReadOnlyLoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }

  public ReadOnlyLoggingSettings() : this(LoggingExtent.ActivityOnly)
  {
  }

  public ReadOnlyLoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  [JsonConstructor]
  public ReadOnlyLoggingSettings(LoggingExtent extent, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
    new LoggingSettingsValidator().ValidateAndThrow(this);
  }
}
