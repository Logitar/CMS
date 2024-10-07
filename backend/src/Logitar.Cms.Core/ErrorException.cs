using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core;

public abstract class ErrorException : Exception
{
  public abstract Error Error { get; }

  protected ErrorException() : this(message: null)
  {
  }
  protected ErrorException(string? message) : this(message, innerException: null)
  {
  }
  protected ErrorException(Exception? innerException) : this(message: null, innerException)
  {
  }
  protected ErrorException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
