namespace Logitar.Cms.Contracts.Languages;

public record UpdateLanguagePayload
{
  public string? Locale { get; set; }
}
