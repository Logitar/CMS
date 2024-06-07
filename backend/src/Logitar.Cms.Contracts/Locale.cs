namespace Logitar.Cms.Contracts;

public record Locale
{
  public int Id { get; set; }
  public string Code { get; set; }
  public string DisplayName { get; set; }
  public string EnglishName { get; set; }
  public string NativeName { get; set; }

  public Locale() : this(string.Empty)
  {
  }

  public Locale(string code) : this(CultureInfo.GetCultureInfo(code))
  {
  }

  public Locale(CultureInfo culture)
  {
    Id = culture.LCID;
    Code = culture.Name;
    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }
}
