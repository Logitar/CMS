namespace Logitar.Cms.Core;

public record CustomAttributeModification
{
  public string Key { get; set; } = string.Empty;
  public string? Value { get; set; }

  public CustomAttributeModification()
  {
  }

  public CustomAttributeModification(KeyValuePair<string, string?> pair) : this(pair.Key, pair.Value)
  {
  }

  public CustomAttributeModification(string key, string? value)
  {
    Key = key;
    Value = value;
  }
}
