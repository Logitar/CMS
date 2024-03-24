namespace Logitar.Cms.Contracts.Localization;

public record CreateLanguagePayload
{
  public string Locale { get; set; }

  public CreateLanguagePayload() : this(string.Empty)
  {
  }

  public CreateLanguagePayload(string locale)
  {
    Locale = locale;
  }
}
