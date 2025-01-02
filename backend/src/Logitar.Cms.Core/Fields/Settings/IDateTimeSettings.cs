namespace Logitar.Cms.Core.Fields.Settings;

public interface IDateTimeSettings
{
  DateTime? MinimumValue { get; }
  DateTime? MaximumValue { get; }
}
