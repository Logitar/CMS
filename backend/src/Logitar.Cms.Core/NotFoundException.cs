namespace Logitar.Cms.Core;

public abstract class NotFoundException : ErrorException
{
  protected NotFoundException() : this(message: null)
  {
  }
  protected NotFoundException(string? message) : this(message, innerException: null)
  {
  }
  protected NotFoundException(Exception? innerException) : this(message: null, innerException)
  {
  }
  protected NotFoundException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
