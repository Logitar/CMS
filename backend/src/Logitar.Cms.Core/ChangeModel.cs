﻿namespace Logitar.Cms.Core;

public record ChangeModel<T>
{
  public T? Value { get; set; }

  public ChangeModel()
  {
  }

  public ChangeModel(T? value)
  {
    Value = value;
  }
}