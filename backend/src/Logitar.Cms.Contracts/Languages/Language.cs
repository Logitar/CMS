namespace Logitar.Cms.Contracts.Languages;

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

  public override string ToString() => $"{Locale.DisplayName} ({Locale.Code}) | {base.ToString()}";
}
