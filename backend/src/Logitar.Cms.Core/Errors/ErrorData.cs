namespace Logitar.Cms.Core.Errors;

public record ErrorData
{
  public string Key { get; init; } = string.Empty;
  public object? Value { get; init; }

  public ErrorData()
  {
  }

  public ErrorData(KeyValuePair<string, object?> pair)
  {
    Key = pair.Key;
    Value = pair.Value;
  }

  public ErrorData(string key, object? value)
  {
    Key = key;
    Value = value;
  }
}
