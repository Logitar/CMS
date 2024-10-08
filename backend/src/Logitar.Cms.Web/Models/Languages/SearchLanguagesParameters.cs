using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Web.Models.Search;

namespace Logitar.Cms.Web.Models.Languages;

public record SearchLanguagesParameters : SearchParameters
{
  public SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();

    FillPayload(payload);

    List<SortOption> sortOptions = ((SearchPayload)payload).Sort;
    foreach (SortOption sort in sortOptions)
    {
      if (Enum.TryParse(sort.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
