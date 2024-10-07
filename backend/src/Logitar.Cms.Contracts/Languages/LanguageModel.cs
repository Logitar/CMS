namespace Logitar.Cms.Contracts.Languages;

public class LanguageModel : AggregateModel
{
  public bool IsDefault { get; set; }
  public LocaleModel Locale { get; set; }

  public LanguageModel() : this(new LocaleModel())
  {
  }

  public LanguageModel(LocaleModel locale)
  {
    Locale = locale;
  }
}
