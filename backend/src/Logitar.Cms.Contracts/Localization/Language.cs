namespace Logitar.Cms.Contracts.Localization;

public class Language : Aggregate
{
  public bool IsDefault { get; set; }
  public Locale Locale { get; set; }

  public Language() : this(new Locale())
  {
  }

  public Language(Locale locale)
  {
    Locale = locale;
  }
}
