using FluentValidation;
using Logitar.Cms.Contracts;

namespace Logitar.Cms.Core.Languages;

public class Locale
{
  public CultureInfo Culture { get; }
  public RegionInfo? Region { get; }

  public string Code => Culture.Name;
  public string LanguageCode => Culture.TwoLetterISOLanguageName;
  public string? RegionCode => Region?.TwoLetterISORegionName;

  public string DisplayName => Culture.DisplayName;
  public string EnglishName => Culture.EnglishName;
  public string NativeName => Culture.NativeName;

  public Locale(string code) : this(CultureInfo.GetCultureInfo(code))
  {
  }
  public Locale(CultureInfo culture)
  {
    Culture = culture;
    Region = culture.GetRegion();

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
