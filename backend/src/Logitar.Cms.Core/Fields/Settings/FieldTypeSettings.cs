namespace Logitar.Cms.Core.Fields.Settings;

public abstract record FieldTypeSettings
{
  public abstract DataType DataType { get; }
}
