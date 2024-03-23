using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Shared;

public record IdentifierUnit
{
  public string Value { get; }

  public IdentifierUnit(string value)
  {
    Value = value.Trim();
    new IdentifierValidator().ValidateAndThrow(value);
  }

  public static IdentifierUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}
