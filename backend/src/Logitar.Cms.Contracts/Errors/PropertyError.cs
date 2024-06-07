namespace Logitar.Cms.Contracts.Errors;

public record PropertyError : Error
{
  public string? PropertyName { get; set; }
  public string? AttemptedValue { get; set; }

  public PropertyError() : base()
  {
  }

  public PropertyError(string code, string message) : base(code, message)
  {
  }

  public PropertyError(string code, string message, IEnumerable<ErrorData> data) : base(code, message, data)
  {
  }
}
