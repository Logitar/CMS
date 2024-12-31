namespace Logitar.Cms.Core;

public record CustomAttribute
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public CustomAttribute()
  {
  }

  public CustomAttribute(KeyValuePair<string, string> pair)
  {
    Key = pair.Key;
    Value = pair.Value;
  }

  public CustomAttribute(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
