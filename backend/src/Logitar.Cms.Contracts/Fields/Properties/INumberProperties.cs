namespace Logitar.Cms.Contracts.Fields.Properties;

public interface INumberProperties
{
  double? MinimumValue { get; }
  double? MaximumValue { get; }
  double? Step { get; }
}
