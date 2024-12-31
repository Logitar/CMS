using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Localization.Models;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption> Sort { get; set; } = [];
}
