using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.Content;

public record SearchContentsParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public Guid? ContentTypeId { get; set; }

  [FromQuery(Name = "language")]
  public Guid? LanguageId { get; set; }

  public SearchContentsPayload ToPayload()
  {
    SearchContentsPayload payload = new()
    {
      ContentTypeId = ContentTypeId,
      LanguageId = LanguageId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ContentSort field))
      {
        payload.Sort.Add(new ContentSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
