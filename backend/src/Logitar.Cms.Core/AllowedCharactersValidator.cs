using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.Cms.Core;

internal class AllowedCharactersValidator<T> : IPropertyValidator<T, string>
{
  public string AllowedCharacters { get; }
  public string Name { get; } = "AllowedCharactersValidator";

  public AllowedCharactersValidator(string allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
  }

  public string GetDefaultMessageTemplate(string errorCode) => $"'{{PropertyName}}' may only include the following characters: {AllowedCharacters}";

  public bool IsValid(ValidationContext<T> context, string value) => value.All(AllowedCharacters.Contains);
}
