namespace Logitar.Cms.Contracts.Errors;

public record ValidationFailure : Error
{
  public string? PropertyName { get; set; }
  public object? AttemptedValue { get; set; }

  public ValidationFailure() : base(string.Empty, string.Empty)
  {
  }

  public ValidationFailure(string code, string message) : base(code, message)
  {
  }
}
