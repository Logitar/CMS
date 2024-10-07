namespace Logitar.Cms.Contracts.Errors;

public record PropertyError : Error
{
  public string? PropertyName { get; set; }
  public object? AttemptedValue { get; set; }

  public PropertyError() : this(string.Empty, string.Empty, null, null)
  {
  }

  public PropertyError(string code, string message, string? propertyName, object? attemptedValue, IEnumerable<ErrorData>? data = null)
    : base(code, message, data)
  {
    PropertyName = propertyName;
    AttemptedValue = attemptedValue;
  }
}
