namespace Logitar.Cms.Core.Fields.Settings;

public interface ISelectOption
{
  bool IsDisabled { get; }
  string Text { get; }
  string? Label { get; }
  string? Value { get; }
}
