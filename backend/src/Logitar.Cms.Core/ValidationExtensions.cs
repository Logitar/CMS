using FluentValidation;
using Logitar.Cms.Core.Languages.Validators;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> DisplayName<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.DisplayName.MaximumLength);
  }

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

  public static IRuleBuilderOptions<T, string> UniqueName<T>(this IRuleBuilder<T, string> ruleBuilder, IUniqueNameSettings settings)
  {
    IRuleBuilderOptions<T, string> options = ruleBuilder.NotEmpty().MaximumLength(Core.UniqueName.MaximumLength);
    if (settings.AllowedCharacters != null)
    {
      options = options.SetValidator(new AllowedCharactersValidator<T>(settings.AllowedCharacters));
    }
    return options;
  }
}
