namespace Logitar.Cms.Contracts.Configurations;

public interface ILoggingSettings
{
  LoggingExtent Extent { get; }
  bool OnlyErrors { get; }
}
