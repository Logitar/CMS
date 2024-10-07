namespace Logitar.Cms.Contracts.Errors;

public record PropertyError : Error
{
  public object? AttemptedValue { get; set; }
  public string? PropertyName { get; set; }

  public PropertyError() : base(string.Empty, string.Empty)
  {
  }

  public PropertyError(string code, string message, object? attemptedValue, string? propertyName)
    : base(code, message)
  {
    AttemptedValue = attemptedValue;
    PropertyName = propertyName;
  }
}
