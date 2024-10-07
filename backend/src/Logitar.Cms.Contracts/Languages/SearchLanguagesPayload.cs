using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.Languages;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption> Sort { get; set; } = [];
}
