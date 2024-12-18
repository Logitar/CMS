namespace Logitar.Cms.Core.Localization.Models;

public class LocaleModel
{
  public int LCID { get; set; }
  public string Code { get; set; } = string.Empty;

  public string DisplayName { get; set; } = string.Empty;
  public string EnglishName { get; set; } = string.Empty;
  public string NativeName { get; set; } = string.Empty;

  public LocaleModel() : this(string.Empty)
  {
  }

  public LocaleModel(string locale) : this(CultureInfo.GetCultureInfo(locale))
  {
  }

  public LocaleModel(CultureInfo culture)
  {
    LCID = culture.LCID;
    Code = culture.Name;

    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }

  public override bool Equals(object? obj) => obj is LocaleModel locale && locale.LCID == LCID;
  public override int GetHashCode() => LCID.GetHashCode();
  public override string ToString() => $"{DisplayName} ({Code})";
}
