namespace Logitar.Cms.Core.Localization.Models;

public record CreateOrReplaceLanguagePayload
{
  public string Locale { get; set; } = string.Empty;

  public CreateOrReplaceLanguagePayload()
  {
  }

  public CreateOrReplaceLanguagePayload(string locale)
  {
    Locale = locale;
  }
}
