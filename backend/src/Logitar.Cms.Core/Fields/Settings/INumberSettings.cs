namespace Logitar.Cms.Core.Fields.Settings;

public interface INumberSettings
{
  double? MinimumValue { get; }
  double? MaximumValue { get; }
  double? Step { get; }
}
