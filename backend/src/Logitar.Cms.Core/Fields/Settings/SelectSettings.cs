namespace Logitar.Cms.Core.Fields.Settings;

public record SelectSettings : FieldTypeSettings, ISelectSettings
{
  public override DataType DataType { get; } = DataType.Select;

  public bool IsMultiple { get; }
  public IReadOnlyCollection<SelectOption> Options { get; }

  [JsonConstructor]
  public SelectSettings(bool isMultiple = false, IReadOnlyCollection<SelectOption>? options = null)
  {
    IsMultiple = isMultiple;
    Options = options ?? [];
  }

  public SelectSettings(ISelectSettings select)
  {
    IsMultiple = select.IsMultiple;
    Options = select.Options;
  }
}
