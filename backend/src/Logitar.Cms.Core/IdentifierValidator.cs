using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.Cms.Core;

internal class IdentifierValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "IdentifierValidator";

  public string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' must not start with a digit and may only contains letters, digits and underscores (_).";

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return !string.IsNullOrEmpty(value) && !char.IsDigit(value.First()) && value.All(c => char.IsLetterOrDigit(c) || c == '_');
  }
}
