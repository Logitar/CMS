namespace Logitar.Cms.Core.Fields.Settings;

public interface ISelectOption
{
  bool IsDisabled { get; }
  bool IsSelected { get; }
  string Text { get; }
  string? Label { get; }
  string? Value { get; }
}
