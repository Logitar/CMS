using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core;

public abstract class ErrorException : Exception
{
  public abstract Error Error { get; }

  public ErrorException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
