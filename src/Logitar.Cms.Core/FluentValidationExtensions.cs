using FluentValidation;
using Logitar.Cms.Core.Configurations;
using System.Globalization;

namespace Logitar.Cms.Core;

internal static class FluentValidationExtensions
{
  public static IRuleBuilder<T, CultureInfo?> Locale<T>(this IRuleBuilder<T, CultureInfo?> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidLocale).WithErrorCode("LocaleValidator")
      .WithMessage("'{PropertyName}' must be an instance of the CultureInfo class with a non-empty name and a LCID different from 4096.");
  }
  internal static bool BeAValidLocale(CultureInfo? locale) => locale == null
    || (!string.IsNullOrWhiteSpace(locale.Name) && locale.LCID != 4096);

  public static IRuleBuilder<T, string?> NullOrNotEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(BeNullOrNotEmpty).WithErrorCode("NullOrNotEmptyValidator")
      .WithMessage("'{PropertyName}' must be null or a non-empty string.");
  }
  internal static bool BeNullOrNotEmpty(string? s) => s == null || !string.IsNullOrWhiteSpace(s);

  public static IRuleBuilder<T, string?> Username<T>(this IRuleBuilder<T, string?> ruleBuilder, IUsernameSettings usernameSettings)
  {
    return ruleBuilder.Must(u => BeAValidUsername(u, usernameSettings))
      .WithErrorCode("UsernameValidator")
      .WithMessage($"'{{PropertyName}}' may only contain the following characters: '{usernameSettings.AllowedCharacters}'.");
  }
  internal static bool BeAValidUsername(string? username, IUsernameSettings usernameSettings)
    => username == null || usernameSettings.AllowedCharacters == null || username.All(usernameSettings.AllowedCharacters.Contains);
}
