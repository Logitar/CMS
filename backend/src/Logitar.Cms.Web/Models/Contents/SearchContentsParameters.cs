using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.Contents;

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

    FillPayload(payload);

    var sortOptions = ((SearchPayload)payload).Sort;
    if (sortOptions != null)
    {
      payload.Sort = new List<ContentSortOption>(capacity: sortOptions.Count);
      foreach (var sort in sortOptions)
      {
        if (Enum.TryParse(sort.Field, out ContentSort field))
        {
          payload.Sort.Add(new ContentSortOption(field, sort.IsDescending));
        }
      }
    }

    return payload;
  }
}
