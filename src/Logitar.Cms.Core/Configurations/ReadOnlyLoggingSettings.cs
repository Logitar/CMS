using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Configurations;

public record ReadOnlyLoggingSettings
{
  public LoggingExtent Extent { get; init; } = LoggingExtent.ActivityOnly;
  public bool OnlyErrors { get; init; }
}
