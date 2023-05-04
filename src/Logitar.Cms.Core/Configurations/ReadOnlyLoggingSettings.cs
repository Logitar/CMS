using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Configurations;

public record ReadOnlyLoggingSettings
{
  public LoggingExtent Extent { get; init; }
  public bool OnlyErrors { get; init; }
}
