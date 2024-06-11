using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.Localization;

public record LanguageSortOption : SortOption
{
  public new LanguageSort Field
  {
    get => Enum.Parse<LanguageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LanguageSortOption() : base()
  {
  }

  public LanguageSortOption(LanguageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
