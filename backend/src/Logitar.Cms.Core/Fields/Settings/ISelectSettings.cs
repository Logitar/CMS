namespace Logitar.Cms.Core.Fields.Settings;

public interface ISelectSettings
{
  bool IsMultiple { get; }
  IReadOnlyCollection<SelectOption> Options { get; }
}
