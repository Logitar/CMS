using FluentValidation;
using Logitar.Cms.Core.ContentTypes.Validators;

namespace Logitar.Cms.Core.ContentTypes;

public record PlaceholderUnit
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public PlaceholderUnit(string value)
  {
    Value = value.Trim();
    new PlaceholderValidator().ValidateAndThrow(Value);
  }

  public static PlaceholderUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}
