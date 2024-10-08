using FluentValidation;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Configurations.Validators;

namespace Logitar.Cms.Core.Configurations;

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }

  public LoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  [JsonConstructor]
  public LoggingSettings(LoggingExtent extent, bool onlyErrors)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
    new LoggingSettingsValidator().ValidateAndThrow(this);
  }
}
