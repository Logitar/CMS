namespace Logitar.Cms.Core;

public abstract class BadRequestException : ErrorException
{
  protected BadRequestException() : this(message: null)
  {
  }
  protected BadRequestException(string? message) : this(message, innerException: null)
  {
  }
  protected BadRequestException(Exception? innerException) : this(message: null, innerException)
  {
  }
  protected BadRequestException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
