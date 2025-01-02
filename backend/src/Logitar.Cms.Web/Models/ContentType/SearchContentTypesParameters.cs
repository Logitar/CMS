using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.ContentType;

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
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ContentTypeSort field))
      {
        payload.Sort.Add(new ContentTypeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
