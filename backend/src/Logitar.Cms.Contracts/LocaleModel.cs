namespace Logitar.Cms.Contracts;

public class LocaleModel
{
  public int LCID { get; set; }

  public string Code { get; set; }
  public string LanguageCode { get; set; }
  public string? RegionCode { get; set; }

  public string DisplayName { get; set; }
  public string EnglishName { get; set; }
  public string NativeName { get; set; }

  public LocaleModel() : this(CultureInfo.InvariantCulture)
  {
  }

  public LocaleModel(string code) : this(CultureInfo.GetCultureInfo(code))
  {
  }

  public LocaleModel(CultureInfo culture)
  {
    LCID = culture.LCID;

    Code = culture.Name;
    LanguageCode = culture.TwoLetterISOLanguageName;
    RegionCode = culture.GetRegion()?.TwoLetterISORegionName;

    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }

  public override bool Equals(object? obj) => obj is LocaleModel locale && locale.LCID == LCID;
  public override int GetHashCode() => LCID.GetHashCode();
  public override string ToString() => $"{DisplayName} ({Code})";
}
