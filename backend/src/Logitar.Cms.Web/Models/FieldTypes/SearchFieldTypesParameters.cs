using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.FieldTypes;

public record SearchFieldTypesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public DataType? DataType { get; set; }

  public SearchFieldTypesPayload ToPayload()
  {
    SearchFieldTypesPayload payload = new()
    {
      DataType = DataType
    };

    FillPayload(payload);

    List<SortOption>? sortOptions = ((SearchPayload)payload).Sort;
    if (sortOptions != null)
    {
      payload.Sort = new List<FieldTypeSortOption>(capacity: sortOptions.Count);
      foreach (SortOption sort in sortOptions)
      {
        if (Enum.TryParse(sort.Field, out FieldTypeSort field))
        {
          payload.Sort.Add(new FieldTypeSortOption(field, sort.IsDescending));
        }
      }
    }

    return payload;
  }
}
