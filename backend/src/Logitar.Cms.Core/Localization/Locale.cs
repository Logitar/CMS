using FluentValidation;

namespace Logitar.Cms.Core.Localization;

public record Locale
{
  public const int MaximumLength = 16;

  public CultureInfo Culture { get; }
  public string Value { get; }

  public Locale(CultureInfo culture)
  {
    Culture = culture;
    Value = culture.Name;
  }
  public Locale(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);

    Culture = CultureInfo.GetCultureInfo(value);
  }

  public override string ToString() => $"{Culture.DisplayName} ({Culture.Name})";

  private class Validator : AbstractValidator<Locale>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Locale();
    }
  }
}
