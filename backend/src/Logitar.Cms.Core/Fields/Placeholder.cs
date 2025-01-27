﻿using FluentValidation;

namespace Logitar.Cms.Core.Fields;

public partial record Placeholder
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Placeholder(string value)
  {
    Value = value.Trim();
  }

  public static Placeholder? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Placeholder>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Placeholder();
    }
  }
}
