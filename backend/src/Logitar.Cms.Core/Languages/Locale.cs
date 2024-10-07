using FluentValidation;

namespace Logitar.Cms.Core.Languages;

public class Locale
{
  public CultureInfo Culture { get; }
  public string Code => Culture.Name;
  public string DisplayName => Culture.DisplayName;

  public Locale(string code) : this(CultureInfo.GetCultureInfo(code))
  {
  }
  public Locale(CultureInfo culture)
  {
    Culture = culture;
    new Validator().ValidateAndThrow(this);
  }

  public override bool Equals(object? obj) => obj is Locale locale && locale.Code == Code;
  public override int GetHashCode() => Code.GetHashCode();
  public override string ToString() => $"{DisplayName} ({Code})";

  public class Validator : AbstractValidator<Locale>
  {
    public Validator()
    {
      RuleFor(x => x.Code).Locale();
    }
  }
}
