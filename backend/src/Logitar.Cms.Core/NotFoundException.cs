using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core;

public abstract class NotFoundException : Exception
{
  public abstract Error Error { get; }

  public NotFoundException() : base()
  {
  }

  public NotFoundException(string? message) : base(message)
  {
  }

  public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
