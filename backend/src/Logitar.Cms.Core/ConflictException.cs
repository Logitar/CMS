namespace Logitar.Cms.Core;

public abstract class ConflictException : ErrorException
{
  public ConflictException(string? message = null, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
