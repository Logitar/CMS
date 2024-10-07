namespace Logitar.Cms.Core;

public abstract class NotFoundException : ErrorException
{
  public NotFoundException(string? message = null, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
