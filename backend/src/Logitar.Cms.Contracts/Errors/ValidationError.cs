namespace Logitar.Cms.Contracts.Errors;

public record ValidationError : Error
{
  public List<ValidationFailure> Failures { get; set; }

  public ValidationError() : this(string.Empty, string.Empty)
  {
  }

  public ValidationError(string code, string message) : base(code, message)
  {
    Failures = [];
  }

  public ValidationError(string code, string message, IEnumerable<ValidationFailure> failures) : this(code, message)
  {
    Failures.AddRange(failures);
  }
}
