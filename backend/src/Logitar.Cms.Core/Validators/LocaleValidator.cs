using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.Cms.Core.Validators;

internal class LocaleValidator<T> : IPropertyValidator<T, string>
{
  private const int LOCALE_CUSTOM_UNSPECIFIED = 0x1000;

  public string Name { get; } = "LocaleValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be a valid culture code. It cannot be the invariant culture, nor a user-defined culture.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      CultureInfo culture = CultureInfo.GetCultureInfo(value);
      return !string.IsNullOrEmpty(culture.Name) && culture.LCID != LOCALE_CUSTOM_UNSPECIFIED;
    }
    catch (CultureNotFoundException)
    {
    }

    return false;
  }
}
