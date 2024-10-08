using FluentValidation;
using Logitar.Cms.Core.Languages.Validators;

namespace Logitar.Cms.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> JwtSecret<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MinimumLength(Configurations.JwtSecret.MinimumLength)
      .MaximumLength(Configurations.JwtSecret.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Locale<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.SetValidator(new LocaleValidator<T>());
  }
}
