namespace Logitar.Cms.Core;

public record CustomIdentifierModel
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public CustomIdentifierModel()
  {
  }

  public CustomIdentifierModel(KeyValuePair<string, string> pair)
  {
    Key = pair.Key;
    Value = pair.Value;
  }

  public CustomIdentifierModel(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
