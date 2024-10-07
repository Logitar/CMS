namespace Logitar.Cms.Core;

public abstract class BadRequestException : ErrorException
{
  public BadRequestException(string? message = null, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
