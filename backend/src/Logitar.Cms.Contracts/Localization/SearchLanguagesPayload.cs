using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.Localization;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption>? Sort { get; set; }
}
