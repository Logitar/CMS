namespace Logitar.Cms.Contracts.Languages;

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
