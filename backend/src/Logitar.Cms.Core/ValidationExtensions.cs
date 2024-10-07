using FluentValidation;
using Logitar.Cms.Core.Languages.Validators;

namespace Logitar.Cms.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Locale<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.SetValidator(new LocalePropertyValidator<T>());
  }
}
