using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.FieldType;

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
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out FieldTypeSort field))
      {
        payload.Sort.Add(new FieldTypeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
