using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.Languages;

public record LanguageSortOption : SortOption
{
  public new LanguageSort Field
  {
    get => Enum.Parse<LanguageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LanguageSortOption() : this(LanguageSort.DisplayName)
  {
  }

  public LanguageSortOption(LanguageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
