namespace Logitar.Cms.Contracts.Errors;

public record InvalidCredentialsError : Error
{
  public InvalidCredentialsError() : this("InvalidCredentials", "The specified credentials did not match.")
  {
  }

  public InvalidCredentialsError(string code, string message, IEnumerable<ErrorData>? data = null)
    : base(code, message, data)
  {
  }
}
