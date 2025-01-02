﻿namespace Logitar.Cms.Core;

public abstract class BadRequestException : ErrorException
{
  protected BadRequestException(string message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
