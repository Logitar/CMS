namespace Logitar.Cms.Core.Fields.Settings;

public interface IStringSettings
{
  int? MinimumLength { get; }
  int? MaximumLength { get; }
  string? Pattern { get; }
}
