namespace Logitar.Cms.Contracts.Languages;

public record CreateOrReplaceLanguagePayload
{
  public string Locale { get; set; }

  public CreateOrReplaceLanguagePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceLanguagePayload(string locale)
  {
    Locale = locale;
  }
}
