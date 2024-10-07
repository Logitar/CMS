namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public interface INumberProperties
{
  double? MinimumValue { get; }
  double? MaximumValue { get; }
  double? Step { get; }
}
