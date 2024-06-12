namespace Logitar.Cms.Contracts.Errors;

public record PropertyError : Error
{
  public string? PropertyName { get; set; }
  public object? AttemptedValue { get; set; }

  public PropertyError() : base()
  {
  }

  public PropertyError(string code, string message) : base(code, message)
  {
  }

  public PropertyError(string code, string message, string? propertyName, object? attemptedValue) : base(code, message)
  {
    PropertyName = propertyName;
    AttemptedValue = attemptedValue;
  }
}
