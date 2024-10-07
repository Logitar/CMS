namespace Logitar.Cms.Core;

public abstract class ConflictException : ErrorException
{
  protected ConflictException() : this(message: null)
  {
  }
  protected ConflictException(string? message) : this(message, innerException: null)
  {
  }
  protected ConflictException(Exception? innerException) : this(message: null, innerException)
  {
  }
  protected ConflictException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
