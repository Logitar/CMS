using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Web.Models.Search;

namespace Logitar.Cms.Web.Models.Languages;

public record SearchLanguagesParameters : SearchParameters
{
  public SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
