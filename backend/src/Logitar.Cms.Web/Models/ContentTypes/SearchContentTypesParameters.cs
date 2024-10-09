using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.ContentTypes;

public record SearchContentTypesParameters : SearchParameters
{
  [FromQuery(Name = "invariant")]
  public bool? IsInvariant { get; set; }

  public SearchContentTypesPayload ToPayload()
  {
    SearchContentTypesPayload payload = new()
    {
      IsInvariant = IsInvariant
    };

    FillPayload(payload);

    List<SortOption> sortOptions = ((SearchPayload)payload).Sort;
    foreach (SortOption sort in sortOptions)
    {
      if (Enum.TryParse(sort.Field, out ContentTypeSort content))
      {
        payload.Sort.Add(new ContentTypeSortOption(content, sort.IsDescending));
      }
    }

    return payload;
  }
}
