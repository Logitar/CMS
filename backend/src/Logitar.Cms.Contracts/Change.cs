﻿namespace Logitar.Cms.Contracts;

public record Change<T>
{
  public T? Value { get; }

  public Change(T? value)
  {
    Value = value;
  }
}
